#ifndef TCCG_UTIL_BLEND
#define TCCG_UTIL_BLEND

inline fixed3 blend_layer1 (fixed3 base, fixed3 layer1, half factor1)
{
	return lerp (base, layer1, factor1);
}

inline fixed3 blend_layer2 (fixed3 base, fixed3 layer1, half factor1, fixed3 layer2, half factor2)
{    
	half currFactor = factor2;
	fixed3 c = layer2 * currFactor;
	half remainingFactor = max (0, 1 - currFactor);

	currFactor = min (factor1, remainingFactor);
	c += layer1 * currFactor;
	remainingFactor = max (0, remainingFactor - currFactor);

	c += base * remainingFactor;

	return c;
}

inline fixed3 blend_layer3 (fixed3 base, fixed3 layer1, half factor1, fixed3 layer2, half factor2, fixed3 layer3, half factor3)
{
	half currFactor = factor3;
	fixed3 c = layer3 * currFactor;
	half remainingFactor = max (0, 1 - currFactor);

	currFactor = min (factor2, remainingFactor);
	c += layer2 * currFactor;
	remainingFactor = max (0, remainingFactor - currFactor);

	currFactor = min (factor1, remainingFactor);
	c += layer1 * currFactor;
	remainingFactor = max (0, remainingFactor - currFactor);

	c += base * remainingFactor;

	return c;
}

#endif  // #ifndef TCCG_UTIL_BLEND
