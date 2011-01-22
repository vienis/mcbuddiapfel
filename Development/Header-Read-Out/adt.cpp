#include "adt.h"
#include <cstdio>



int main()
{
	
	
	FILE* input = fopen("a.adt", "rb");
	if(!input)
		return 1;
	fseek(input, 0, SEEK_END);
	unsigned int size = ftell(input);
	fseek(input, 0, SEEK_SET);

	char* Buffer = new char[size];
	fread(Buffer, 1, size, input);
	fclose(input);
	
	MVER* ver = (MVER *)(Buffer);

	printf("MVER: \n");
	printf("Signatur: \t 0x%x \n", ver->signature);
	printf("Size: \t\t 0x%x \n", ver->size);
	printf("Version: \t 0x%x \n", ver->version);

	MHDR* hdr = (MHDR *)(Buffer + 0xC + 0x4);

	printf("MHDR: \n");
	printf("size: \t 0x%x \n", hdr->size);
	printf("flag: \t 0x%x \n", hdr->flag);
	printf("mcin: \t 0x%x \n", hdr->mcin);
	printf("mtex: \t 0x%x \n", hdr->mtex);
	printf("mmdx: \t 0x%x \n", hdr->mmdx);
	printf("mmid: \t 0x%x \n", hdr->mmid);
	printf("mwmo: \t 0x%x \n", hdr->mwmo);
	printf("mwid: \t 0x%x \n", hdr->mwid);
	printf("mddf: \t 0x%x \n", hdr->mddf);
	printf("modf: \t 0x%x \n", hdr->modf);
	printf("mfbo: \t 0x%x \n", hdr->mfbo);
	printf("mh2o: \t 0x%x \n", hdr->mh2o);
	printf("mftx: \t 0x%x \n", hdr->mftx);
	printf("pad4: \t 0x%x \n", hdr->pad4);
	printf("pad5: \t 0x%x \n", hdr->pad5);
	printf("pad6: \t 0x%x \n", hdr->pad6);
	printf("pad7: \t 0x%x \n", hdr->pad7);
	
	return 0;
}