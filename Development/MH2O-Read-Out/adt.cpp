#include "adt.h"
#include <cstdio>

int main()
{
	FILE* input = fopen("a.adt", "rb");
	if(!input)
		return -1;
	fseek(input, 0, SEEK_END);
	unsigned int size = ftell(input);
	fseek(input, 0, SEEK_SET);

	char* buffer = new char[size];
	fread(buffer, 1, size, input);
	fclose(input);

	MHDR* hdr = (MHDR *)(buffer + 0x14);
	MH2O* h2o = (MH2O *)(buffer	+ 0x14 + hdr->mh2o + 0x4);
	MH2O_Info* h2o_info = (MH2O_Info *)(buffer + 0x14 + hdr->mh2o + 0x4 + h2o->Information); 

	printf("MH2O: \n");
	printf("Info: \t\t\t\t 0x%x \n", h2o->Information);
	printf("LayerCount: \t\t\t 0x%x \n", h2o->layerCount);
	printf("Render: \t\t\t 0x%x \n", h2o->Render);
	printf("MH2O_Info: \n");
	printf("Liquidtype: \t\t\t 0x%x \n", h2o_info->LiquidType);
	printf("Flags: \t\t\t\t 0x%x \n", h2o_info->flags);
	printf("HeightLevel1: \t\t\t 0x%x \n", h2o_info->heightLevel1);
	printf("HeightLevel2: \t\t\t 0x%x \n", h2o_info->heightLevel2);
	printf("xOffset: \t\t\t 0x%x \n", h2o_info->xOffset);
	printf("yOffset: \t\t\t 0x%x \n", h2o_info->yOffset);
	printf("Width: \t\t\t\t 0x%x \n", h2o_info->width);
	printf("Height: \t\t\t 0x%x \n", h2o_info->height);
	printf("Mask2: \t\t\t\t 0x%x \n", h2o_info->Mask2);
	printf("HeightmapData: \t\t\t 0x%x \n", h2o_info->HeightmapData);
	if(h2o_info->flags != 1)
	{
		printf("Cause MH2O->flags is not 1, then it is an ocean, so there is no struct needed");
	}
	else
	{
		MH2O_HeightmapData* h2o_hd = (MH2O_HeightmapData *)(buffer + 0x14 + hdr->mh2o + 0x4 + h2o->Information + h2o_info->HeightmapData);
		printf("MH2O_HeightmapData: \n");
		printf("HeightMap: \t\t\t 0x%x \n", h2o_hd->heightMap);
		printf("Transparency: \t\t\t 0x%x \n", h2o_hd->transparency);
	}
	return 0;
}