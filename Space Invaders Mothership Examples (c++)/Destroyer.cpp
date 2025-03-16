#include "Destroyer.h"


CDestroyer::CDestroyer()
{
}


CDestroyer::~CDestroyer()
{
}

bool CDestroyer::Initialise() {
	VALIDATE(CEntity::Initialise(IDB_DESTROYER, IDB_DESTROYER_MASK));

	return (true);
}

void CDestroyer::Draw() {
	CInvader::Draw();
}

void CDestroyer::Process(float _fDeltaTick) {
	CInvader::Process(_fDeltaTick);
}

CBullet* CDestroyer::FireBullet() {
	int _iRandomX = rand() % 200 - 100;
	CBullet* _pBullet = new CThunder;
	_pBullet->Initialise(m_fX, m_fY + 50, _iRandomX, 300);
	return _pBullet;
}
