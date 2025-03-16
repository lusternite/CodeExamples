#include "Blast.h"


CBlast::CBlast()
{
}


CBlast::~CBlast()
{
}

bool CBlast::Initialise(float _fPosX, float _fPosY, float _fVelocityX, float _fVelocityY) {
	VALIDATE(CEntity::Initialise(IDB_INVADER_BLAST, IDB_INVADER_BLAST_MASK));

	m_fX = _fPosX;
	m_fY = _fPosY;

	m_fVelocityX = _fVelocityX;
	m_fVelocityY = _fVelocityY;

	return (true);
}