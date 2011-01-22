#pragma once

typedef unsigned long long uint64;
typedef unsigned int uint32; 
typedef unsigned short uint16;
typedef unsigned char byte;

struct MVER
{
   uint32 signature;
   uint32 size;
   uint32 version;
};

struct MHDR
{
        /*unsigned int size;
        unsigned int flag;*/
        unsigned int mcin;
        unsigned int mtex;
        unsigned int mmdx;
        unsigned int mmid;
        unsigned int mwmo;
        unsigned int mwid;
        unsigned int mddf;
        unsigned int modf;
        unsigned int mfbo;
        unsigned int mh2o;
        unsigned int mftx;
        unsigned int pad4;
        unsigned int pad5;
        unsigned int pad6;
        unsigned int pad7;
};

struct MH2O_HeightmapData
{
	// if type & 1 != 1, this chunk is "ocean".  in this case, do not use this structure.

	float heightMap; 	// w*h
	char transparency; 	// w*h
};

struct MH2O_Info
{
	uint16 LiquidType;
	uint16 flags;
	float heightLevel1;
	float heightLevel2;
	byte xOffset;
	byte yOffset;
	byte width;
	byte height;
	byte Mask2;				// points to an array of bits with information about the mask. w*h bits (=h bytes).
	uint32 HeightmapData;	// note that if flags & 1 != 1, this chunk is "ocean" and the information stored
						// at HeightmapData does not correspond to the MH2O_HeightmapData structure.
						// Use heightLevel1 (or 2) for height in this case.
};

struct MH2O
{
	uint32 Information;		// points to an array with layers entries.
	uint32 layerCount;
	uint64 *Render;				// the blocks to render. 8*8 bits (=8 bytes). regardless of the information on width and height.
};
