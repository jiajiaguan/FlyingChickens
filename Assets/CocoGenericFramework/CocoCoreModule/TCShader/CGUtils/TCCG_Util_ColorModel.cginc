#ifndef TCCG_UTIL_COLOR
#define TCCG_UTIL_COLOR

// HSL ------------------------------------------------

inline half3 rgb_to_hsl(half3 RGB)
{
	half3 HSL;

	half minChannel, maxChannel;
	if (RGB.x > RGB.y) {
		maxChannel = RGB.x;
		minChannel = RGB.y;
	} else {
		maxChannel = RGB.y;
		minChannel = RGB.x;
	}

	if (RGB.z > maxChannel) maxChannel = RGB.z;
	if (RGB.z < minChannel) minChannel = RGB.z;
	
	// L
	HSL.xy = 0;
	HSL.z = (maxChannel + minChannel) / 2;

	// S
	half delta = maxChannel - minChannel;
	if (delta != 0) { 
		if (HSL.z < 0.5) {
			HSL.y = delta / (maxChannel + minChannel);
		} else {
			HSL.y = delta / (2.0 - maxChannel - minChannel);
		}
		
		// H
		half3 del_rgb = ((maxChannel - RGB) / 6.0 + delta / 2.0) / delta;
		if (RGB.x == maxChannel)
			HSL.x = del_rgb.z - del_rgb.y;
		else if (RGB.y == maxChannel)
			HSL.x = 1.0 / 3.0 + del_rgb.x - del_rgb.z;
		else
			HSL.x = 2.0 / 3.0 + del_rgb.y - del_rgb.x;

//		delta *= 6.0;
//		if (RGB.x == maxChannel)
//			HSL.x = (RGB.y - RGB.z) / delta;
//		else if (RGB.y == maxChannel)
//			HSL.x = (2.0 + RGB.z - RGB.x) / delta;
//		else
//			HSL.x = (4.0 + RGB.x - RGB.y) / delta;

		if (HSL.x < 0)
			HSL.x += 1;
		else if (HSL.x > 1)
			HSL.x -= 1;
	}
	
	return (HSL);
}

inline half h_to_rgb (half t1, half t2, half h)
{
	if (h < 0)
		h += 1;
	if (h > 1)
		h -= 1;
	if (6.0 * h < 1)
		return t1 + (t2 - t1) * 6.0 * h;
	if (2.0 * h < 1)
		return t2;
	if (3.0 * h < 2)
		return t1 + (t2 - t1) * ((2.0 / 3.0) - h) * 6.0;
    return (t1);
}

inline half3 hsl_to_rgb (half3 HSL)
{
	half3 RGB = half3 (HSL.z, HSL.z, HSL.z);
	
	half temp1, temp2;
	if (HSL.y != 0) {
		if (HSL.z < 0.5)
			temp2 = HSL.z * (1 + HSL.y);
		else
			temp2 = (HSL.z + HSL.y) - (HSL.y * HSL.z);

        temp1 = 2.0 * HSL.z - temp2;

        RGB.x = h_to_rgb (temp1, temp2, HSL.x + (1.0 / 3.0));
        RGB.y = h_to_rgb (temp1, temp2, HSL.x);
        RGB.z = h_to_rgb (temp1, temp2, HSL.x - (1.0 / 3.0));
	}
	
	return RGB;
}

// blend hsl ------------------

inline half3 blend_hsl_to_rgb (half3 col, half h, half s, half l)
{
	// S
	half3 hsl = rgb_to_hsl (col);
	if (s >= 0) {
		if (s >= 1)
			s = 0.99999;
		half alpha;
		if (s + hsl.y >= 1 && hsl.y > 0)
			alpha = hsl.y;
		else
			alpha = 1 - s;
		alpha = 1 / alpha - 1;
		col = col + (col - hsl.z) * alpha;
	} else {
		col = hsl.z + (col - hsl.z) * (1 + s);
	}
	
	// L
	if (l >= 0) {
		col = l + col * (1 - l);
	} else {
		col *= (1 + l);
	}
	
	// H
	hsl = rgb_to_hsl (col);
	hsl.x +=h;
	
	return hsl_to_rgb (hsl);
}

// HSV ------------------------------------------------

inline half3 rgb_to_hsv(half3 RGB)
{
	half3 HSV;

	half minChannel, maxChannel;
	if (RGB.x > RGB.y) {
		maxChannel = RGB.x;
		minChannel = RGB.y;
	} else {
		maxChannel = RGB.y;
		minChannel = RGB.x;
	}

	if (RGB.z > maxChannel) maxChannel = RGB.z;
	if (RGB.z < minChannel) minChannel = RGB.z;

	HSV.xy = 0;
	HSV.z = maxChannel;
	half delta = maxChannel - minChannel;             //Delta RGB value
	if (delta != 0) {                    // If gray, leave H  S at zero
		HSV.y = delta / HSV.z;
		half3 delRGB;
		delRGB = (HSV.zzz - RGB + 3*delta) / (6.0*delta);
		if      ( RGB.x == HSV.z ) HSV.x = delRGB.z - delRGB.y;
		else if ( RGB.y == HSV.z ) HSV.x = ( 1.0/3.0) + delRGB.x - delRGB.z;
		else if ( RGB.z == HSV.z ) HSV.x = ( 2.0/3.0) + delRGB.y - delRGB.x;
	}
	return (HSV);
}

inline half3 hsv_to_rgb(half3 HSV)
{
	half3 RGB = HSV.z;

	half var_h = HSV.x * 6;
	half var_i = floor(var_h);
	half var_1 = HSV.z * (1.0 - HSV.y);
	half temp2 = HSV.z * (1.0 - HSV.y * (var_h-var_i));
	half var_3 = HSV.z * (1.0 - HSV.y * (1-(var_h-var_i)));

	if (var_i == 0) {
	    RGB = half3(HSV.z, var_3, var_1);
	} else if (var_i == 1) {
	    RGB = half3(temp2, HSV.z, var_1);
	} else if (var_i == 2) {
	    RGB = half3(var_1, HSV.z, var_3);
	} else if (var_i == 3) {
	    RGB = half3(var_1, temp2, HSV.z);
	} else if (var_i == 4) {
	    RGB = half3(var_3, var_1, HSV.z);
	} else {
	    RGB = half3(HSV.z, var_1, temp2);
	}

	return (RGB);
}

// replace hue ----------------

inline half3 replace_hue_by_hsv (half3 col, half hue)
{
	half3 col_hsv = rgb_to_hsv (col);
	col_hsv.x = hue;
	return hsv_to_rgb (col_hsv);
}

inline half3 replace_hue_by_hsv_fix (half3 col, half hue, half saturMin, half saturRatio, half saturAdd, half _lightMax, half _lightRatiio, half _lightAdd)
{
	half3 col_hsv = rgb_to_hsv (col);

	if(col_hsv.y < saturMin)
		col_hsv.y = col_hsv.y * saturRatio + saturAdd;

	if(col_hsv.z > _lightMax)
		col_hsv.z = col_hsv.z * _lightRatiio + _lightAdd;

	col_hsv.x = hue;
	return hsv_to_rgb (col_hsv);
}

#endif  // #ifndef TCCG_UTIL_COLOR
