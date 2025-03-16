// Library Includes

// Local Includes
#include "resource.h"
#include "utils.h"

// This Include
#include "Invader.h"

// Static Variables

// Static Function Prototypes

// Implementation

CInvader::CInvader()
: m_bHit(false)
{

}

CInvader::~CInvader()
{

}

bool
CInvader::Initialise()
{
	VALIDATE(CEntity::Initialise(IDB_SPACEINVADER1, IDB_SPACEINVADER_MASK1));

	return (true);
}

void
CInvader::Draw()
{
	if (!m_bHit)
	{
		CEntity::Draw();
	}
}

void
CInvader::Process(float _fDeltaTick)
{
	if (!m_bHit)
	{
		CEntity::Process(_fDeltaTick);
	}
}

void
CInvader::SetHit(bool _b)
{
	m_bHit = _b;
}

bool
CInvader::IsHit() const
{
	return (m_bHit);
}

CBullet* CInvader::FireBullet() {
	CBullet* _pBullet = new CThunder;
	_pBullet->Initialise(m_fX, m_fY + 50, 0, 200);
	return _pBullet;
}

bool CInvader::TakeDamage() {
	return false;
}

int CInvader::GetHealth() {
	return 0;
}