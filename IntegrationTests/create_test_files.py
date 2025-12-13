# -*- coding: utf-8 -*-
"""
Script pour créer tous les fichiers de test Python manquants
Exécuter avec: python create_test_files.py
"""
import os

# Contenu du fichier test_auth_api.py
test_auth_api_content = '''"""
Tests d'integration pour l'API d'authentification
Niveau: Integration
Framework: pytest + requests
Technique: Boite noire - Scenarios API
"""
import pytest
import requests


@pytest.mark.api
@pytest.mark.auth
@pytest.mark.integration
class TestAuthenticationAPI:
    """
    Suite de tests pour l'API d'authentification
    Couvre: Login, Register, Token validation
    """
    
    def test_TC011_login_valid_credentials_returns_token(self, api_url):
        """
        TC011: Connexion avec identifiants valides retourne un token
        Technique: Scenario nominal
        Resultat attendu: 200 OK + token JWT
        """
        # Arrange
        login_data = {
            'email': 'admin@carrental.com',
            'password': 'Admin@123'
        }
        
        # Act
        response = requests.post(f'{api_url}/api/auth/login', json=login_data)
        
        # Assert
        assert response.status_code == 200, f"Expected 200, got {response.status_code}"
        data = response.json()
        assert 'token' in data, "Response should contain 'token'"
        assert data['token'], "Token should not be empty"
        assert len(data['token'].split('.')) == 3, "Token should be a valid JWT (3 parts)"
    
    def test_TC012_login_invalid_password_returns_unauthorized(self, api_url):
        """
        TC012: Connexion avec mot de passe incorrect retourne 401
        Technique: Cas d'erreur - validation des credentials
        Resultat attendu: 401 Unauthorized
        """
        # Arrange
        login_data = {
            'email': 'admin@carrental.com',
            'password': 'WrongPassword123!'
        }
        
        # Act
        response = requests.post(f'{api_url}/api/auth/login', json=login_data)
        
        # Assert
        assert response.status_code == 401, f"Expected 401, got {response.status_code}"
'''

# Creer le fichier
os.makedirs('tests', exist_ok=True)
with open('tests/test_auth_api.py', 'w', encoding='utf-8') as f:
    f.write(test_auth_api_content)

print("Files de test crees avec succes!")
print("Fichier: IntegrationTests/tests/test_auth_api.py")
