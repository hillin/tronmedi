__constant sampler_t sampler =
      CLK_NORMALIZED_COORDS_FALSE
    | CLK_ADDRESS_CLAMP_TO_EDGE
    | CLK_FILTER_NEAREST;

__kernel void CalculateContrast (
    __read_only image2d_t input,
    __write_only image2d_t output)
{
    const int2 pos = { get_global_id(0), get_global_id(1) };
	const float4 currentPixel = read_imagef(input, sampler, pos);

    float4 sum = 0.0f;
    for(int y = -1; y <= 1; y++) {
        for(int x = -1; x <= 1; x++) {
            float4 constract = fabs(read_imagef(input, sampler, pos + (int2)(x, y)) - currentPixel);
			float4 average = (constract.x + constract.y + constract.z + constract.w) / 4.0f;
			sum += (float4)average;
        }
    }

    write_imagef (output, (int2)(pos.x, pos.y), sum);
}