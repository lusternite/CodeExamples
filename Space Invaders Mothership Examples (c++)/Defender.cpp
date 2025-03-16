// Library Includes

// Local Includes
#include "resource.h"
#include "utils.h"

// This Include
#include "Defender.h"

// Static Variables

// Static Function Prototypes

// Implementation

CDefender::CDefender()
{

}

CDefender::~CDefender()
{

}

bool
CDefender::Initialise()
{
	VALIDATE(CEntity::Initialise(IDB_DEFENDER, IDB_DEFENDER_MASK));
	m_fMovementSpeed = 400.0f;
	m_bHasThunder = false;
	m_bHasWeave = false;
	m_bHasBlast = false;
	return (true);
}

void
CDefender::Draw()
{
	CEntity::Draw();
}

void
CDefender::Process(float _fDeltaTick)
{

	float fHalfPaddleW = m_pSprite->GetWidth() / 2.0f;

	if (GetAsyncKeyState(VK_RIGHT) & 0x8000)
	{
		m_fX += m_fMovementSpeed * _fDeltaTick;
	}
	else if (GetAsyncKeyState(VK_LEFT) & 0x8000)
	{
		m_fX -= m_fMovementSpeed * _fDeltaTick;
	}
	if (m_fX - fHalfPaddleW <= 0)
	{
		m_fX = fHalfPaddleW;
	}
	else if (m_fX + fHalfPaddleW >= 1064)
	{
		m_fX = 1064 - fHalfPaddleW;
	}

	CEntity::Process(_fDeltaTick);
}

float CDefender::GetMovementSpeed() {
	return m_fMovementSpeed;
}

void CDefender::SetMovementSpeed(float _fMS) {
	m_fMovementSpeed = _fMS;
}

void CDefender::GainThunder() {
	m_bHasThunder = true;
}
void CDefender::GainWeave() {
	m_bHasWeave = true;
}
void CDefender::GainBlast() {
	m_bHasBlast = true;
}

bool CDefender::HasThunder() {
	return m_bHasThunder;
}

bool CDefender::HasWeave() {
	return m_bHasWeave;
}

bool CDefender::HasBlast() {
	return m_bHasBlast;
}