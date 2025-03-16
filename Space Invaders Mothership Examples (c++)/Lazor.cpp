#include "Lazor.h"


CLazor::CLazor()
{
}


CLazor::~CLazor()
{
}

bool CLazor::Initialise(float _fPosX, float _fPosY, float _fVelocityX, float _fVelocityY) {
	VALIDATE(CEntity::Initialise(IDB_INVADER_LAZOR, IDB_INVADER_LAZOR_MASK));

	m_fX = _fPosX;
	m_fY = _fPosY;

	m_fVelocityX = 0;
	m_fVelocityY = _fVelocityY;

	m_fLifeTimer = 0.0f;
	m_fLiveForSeconds = 1.0f;

	return (true);
}

CBullet* CLazor::SpawnTail(float _fDeltaTick) {
 	CBullet* _pTail = new CLazor;

	if (m_fVelocityY > 0) {
		_pTail->Initialise(m_fX, m_fY - (m_fVelocityY * _fDeltaTick), m_fVelocityX, m_fVelocityY);
	}
	else {
		_pTail->Initialise(m_fX, m_fY + (m_fVelocityY * _fDeltaTick), m_fVelocityX, m_fVelocityY);
	}

	_pTail->SetLiveForSeconds(m_fLiveForSeconds - m_fLifeTimer);

	m_bSpawnedTail = true;
	
	return _pTail;
}

void CLazor::Process(float _fDeltaTick) {
	m_fX += m_fVelocityX * _fDeltaTick;
	m_fY += m_fVelocityY * _fDeltaTick;

	m_fLifeTimer += _fDeltaTick;

	CEntity::Process(_fDeltaTick);
}

float CLazor::GetLifeTimer() {
	return m_fLifeTimer;
}

void CLazor::SetLiveForSeconds(float _fLiveForSeconds) {
	m_fLiveForSeconds = _fLiveForSeconds;
}

float CLazor::GetLiveForSeconds() {
	return m_fLiveForSeconds;
}

bool CLazor::HasSpawnedTail() {
	return m_bSpawnedTail;
}

bool CLazor::IsLazor() {
	return true;
}

bool CLazor::CanSpawnTail() {
	if (m_fLifeTimer > 0 &&
		m_fLiveForSeconds > 0 &&
		m_bSpawnedTail == false) {
		return true;
	}
	return false;
}