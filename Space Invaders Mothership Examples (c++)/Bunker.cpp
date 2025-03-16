#include "Bunker.h"


CBunker::CBunker()
{
	m_iHealth = 4;
	m_bDestroyed = false;
}


CBunker::~CBunker()
{
}


bool CBunker::Initialise(float _fX, float _fY) {
	m_pBunkerFull = new CSprite;
	m_pBunkerDamaged = new CSprite;
	VALIDATE(m_pBunkerFull->Initialise(IDB_BUNKER_FULL, IDB_BUNKER_FULL_MASK));
	VALIDATE(m_pBunkerDamaged->Initialise(IDB_BUNKER_DAMAGED, IDB_BUNKER_DAMAGED_MASK));
	m_pSprite = m_pBunkerFull;
	SetX(_fX);
	SetY(_fY);
	m_pSprite->SetX(_fX);
	m_pSprite->SetY(_fY);
	m_pBunkerFull->SetX(_fX);
	m_pBunkerFull->SetY(_fY);
	m_pBunkerDamaged->SetX(_fX);
	m_pBunkerDamaged->SetY(_fY);
	return true;
}

void CBunker::Draw() {
	if (!IsDestroyed()) {
		CEntity::Draw();
	}
}

void CBunker::Process(float _fDeltaTick) {
	if (!IsDestroyed()) {
		CEntity::Process(_fDeltaTick);
	}
}

void CBunker::TakeDamage() {
	m_iHealth -= 1;
	if (m_iHealth <= 0) {
		m_bDestroyed = true;
	}
	else if (m_iHealth == 2) {
		m_pSprite = m_pBunkerDamaged;
	}
}

int CBunker::GetHealth() {
	return m_iHealth;
}

void CBunker::SetHealth(int _iHealth) {
	m_iHealth = _iHealth;
}

bool CBunker::IsDestroyed() {
	return m_bDestroyed;
}

void CBunker::SetDestroyed(bool _bDestroyed) {
	m_bDestroyed = _bDestroyed;
}