#include "Mothership.h"


CMothership::CMothership()
{
}


CMothership::~CMothership()
{
}

bool CMothership::Initialise() {
	VALIDATE(CEntity::Initialise(IDB_MOTHERSHIP, IDB_MOTHERSHIP_MASK));

	m_iHealth = 100;

	return (true);
}

CBullet* CMothership::FireBullet() {
	int _iRandom = rand() % 4;
	switch (_iRandom) {
	case 0:
	{
			  int _iRandomX = rand() % 200 - 100;
			  CBullet* _pBullet = new CThunder;
			  _pBullet->Initialise(m_fX, m_fY + 50, _iRandomX, 450);
			  return _pBullet;
			  break;
	}
	case 1:
	{
			  CBullet* _pBullet = new CWeave;
			  _pBullet->Initialise(m_fX, m_fY + 50, 0, 100);
			  return _pBullet;
			  break;
	}
	case 2:
	{
			  CBullet* _pBullet = new CBlast;
			  _pBullet->Initialise(m_fX, m_fY + 50, 0, 600);
			  return _pBullet;
			  break;
	}
	case 3:
	{
			  CBullet* _pBullet = new CLazor;
			  _pBullet->Initialise(m_fX, m_fY + 50, 0, 400);
			  return _pBullet;
			  break;
	}
	default:
		break;
	}
	return NULL;
}

bool CMothership::TakeDamage() {
	m_iHealth -= 1;
	if (m_iHealth <= 0) {
		CGame::GetInstance().GameOverWon();
		return true;
	}
	return false;
}

int CMothership::GetHealth() {
	return m_iHealth;
}