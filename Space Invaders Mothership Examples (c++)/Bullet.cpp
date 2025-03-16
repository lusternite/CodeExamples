// Library Includes

// Local Includes

// This Includes
#include "Bullet.h"

// Static Variables

// Static Function Prototypes

// Implementation

CBullet::CBullet()
: m_fVelocityX(0.0f)
, m_fVelocityY(0.0f)
, m_bDeletion(false)
, m_bBounce(false)
{

}

CBullet::~CBullet()
{

}

bool
CBullet::Initialise(float _fPosX, float _fPosY, float _fVelocityX, float _fVelocityY)
{
	VALIDATE(CEntity::Initialise(IDB_DEFENDER_BULLET, IDB_DEFENDER_BULLET));

	m_fX = _fPosX;
	m_fY = _fPosY;

	m_fVelocityX = _fVelocityX;
	m_fVelocityY = _fVelocityY;

	return (true);
}

void
CBullet::Draw()
{
	CEntity::Draw();
}

void
CBullet::Process(float _fDeltaTick)
{
	m_fX += m_fVelocityX * _fDeltaTick;
	m_fY += m_fVelocityY * _fDeltaTick;

	CEntity::Process(_fDeltaTick);
}

void CBullet::SetLiveForSeconds(float _fLiveForSeconds) {

}

float
CBullet::GetVelocityX() const
{
	return (m_fVelocityX);
}

float
CBullet::GetVelocityY() const
{
	return (m_fVelocityY);
}

void
CBullet::SetVelocityX(float _fX)
{
	m_fVelocityX = _fX;
}

void
CBullet::SetVelocityY(float _fY)
{
	m_fVelocityY = _fY;
}

float
CBullet::GetRadius() const
{
	return (GetWidth() / 2.0f);
}

void CBullet::SetDeletion(bool _bDeletion) {
	m_bDeletion = _bDeletion;
}

bool CBullet::IsDeletion() {
	return m_bDeletion;
}

bool CBullet::IsBounce() {
	return m_bBounce;
}

bool CBullet::IsLazor() {
	return false;
}

CBullet* CBullet::SpawnTail(float _fDeltaTick) {
	return NULL;
}

bool CBullet::CanSpawnTail() {
	return false;
}