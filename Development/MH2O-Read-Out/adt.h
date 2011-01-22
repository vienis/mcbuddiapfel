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

struct MCIN // 03-29-2005 By ObscuR
{
/*000h*/  unsigned int mcnk;                            // absolute offset.
/*004h*/  unsigned int size;                            // the size of the MCNK chunk, this is refering to.
/*008h*/  unsigned int Unused_flags;                    // these two are always 0. only set in the client.
/*00Ch*/  unsigned int Unused_asyncId;
/*010h*/                
};

struct Vec3f
{
        float x, y, z;
};

struct MCNK // --schlumpf_ 17:01, 10 August 2009 (CEST), based on: 03-29-2005 By ObscuR, 11-08-2008 by Tharo
{
/*0x000*/  unsigned int flags;
/*0x004*/  unsigned int IndexX;
/*0x008*/  unsigned int IndexY;
/*0x00C*/  unsigned int nLayers;                                // maximum 4
/*0x010*/  unsigned int nDoodadRefs;
/*0x014*/  unsigned int mcvt;                             //ofsHeight
/*0x018*/  unsigned int mcnr;                             //ofsNormal
/*0x01C*/  unsigned int mcly;                             //ofsLayer
/*0x020*/  unsigned int mcrf;                             //ofsRefs
/*0x024*/  unsigned int mcal;                             //ofsAlpha
/*0x028*/  unsigned int sizeAlpha;
/*0x02C*/  unsigned int mcsh;                           // Shadow //only with flags&0x1
/*0x030*/  unsigned int sizeShadow;
/*0x034*/  unsigned int areaid;
/*0x038*/  unsigned int nMapObjRefs;
/*0x03C*/  unsigned int holes;
/*0x040*/  uint16 ReallyLowQualityTextureingMap[8];     // the content is the layer being on top, I guess.
/*0x044*/
/*0x048*/
/*0x04C*/
/*0x050*/  unsigned int predTex;                                // 03-29-2005 By ObscuR; TODO: Investigate
/*0x054*/  unsigned int noEffectDoodad;                         // 03-29-2005 By ObscuR; TODO: Investigate
/*0x058*/  unsigned int ofsSndEmitters;
/*0x05C*/  unsigned int nSndEmitters;                           //will be set to 0 in the client if ofsSndEmitters doesn't point to MCSE!
/*0x060*/  unsigned int ofsLiquid;
/*0x064*/  unsigned int sizeLiquid;                     // 8 when not used; only read if >8.
/*0x068*/  Vec3f position;
/*0x06C*/  
/*0x070*/ 
/*0x074*/  unsigned int ofsMCCV;                                // only with flags&0x40, had UINT32 textureId; in ObscuR's structure.
/*0x078*/  unsigned int Unused_pad1;                            // will most likely be ofsNewSubChunk in next expansion, currently 0.
/*0x07C*/  unsigned int Unused_pad2;                            // most likely nNewSubChunkEntries/sizeNewSubChunk in next Expansion or another offset --Tigurius
/*0x080*/
};

enum // 03-29-2005 By ObscuR
{
        FLAG_MCSH,
        FLAG_IMPASS,
        FLAG_LQ_RIVER,
        FLAG_LQ_OCEAN,
        FLAG_LQ_MAGMA,
        FLAG_MCCV,
};
