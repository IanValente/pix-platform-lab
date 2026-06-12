package com.pixplatform.pixservice.infrastructure.adapter.out;

import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;
import java.util.UUID;

@Repository
public interface PixJpaRepository extends JpaRepository<PixEntity, UUID> {
}