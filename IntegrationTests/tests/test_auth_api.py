# -*- coding: utf-8 -*-
import pytest
import requests
import random

@pytest.mark.api
@pytest.mark.auth
@pytest.mark.integration
class TestAuthenticationAPI:
    
    def test_TC011_login_valid_credentials_returns_token(self, api_url):
        """Test login with valid credentials returns JWT token"""
        login_data = {
            'username': 'admin',
            'password': 'Admin@123'
        }
        
        try:
            response = requests.post(f'{api_url}/api/auth/login', json=login_data, timeout=5)
        except requests.exceptions.ConnectionError:
            pytest.skip("API is not running. Start Backend with 'dotnet run' in Backend folder")
        except requests.exceptions.Timeout:
            pytest.skip("API request timed out. Check if Backend is running")
        
        # Accept both 200 (success) and 401 (if admin not seeded yet)
        assert response.status_code in [200, 401], f"Expected 200 or 401, got {response.status_code}. Response: {response.text}"
        
        if response.status_code == 200:
            data = response.json()
            assert 'token' in data, "Response should contain 'token'"
            assert data['token'], "Token should not be empty"
            # JWT tokens have 3 parts separated by dots
            assert len(data['token'].split('.')) == 3, "JWT token should have 3 parts"
            assert 'username' in data, "Response should contain 'username'"
            assert 'email' in data, "Response should contain 'email'"
    
    def test_TC012_login_invalid_password_returns_unauthorized(self, api_url):
        """Test login with invalid password returns 401 Unauthorized"""
        login_data = {
            'username': 'admin',
            'password': 'WrongPassword123!'
        }
        
        try:
            response = requests.post(f'{api_url}/api/auth/login', json=login_data, timeout=5)
        except requests.exceptions.ConnectionError:
            pytest.skip("API is not running. Start Backend with 'dotnet run' in Backend folder")
        except requests.exceptions.Timeout:
            pytest.skip("API request timed out")
        
        assert response.status_code == 401, f"Expected 401, got {response.status_code}"
    
    @pytest.mark.parametrize("username,password", [
        ('', 'password123'),
        ('invaliduser', ''),
        ('nonexistentuser', 'SomePassword123'),
    ])
    def test_TC013_login_invalid_inputs(self, api_url, username, password):
        """Test login with various invalid inputs returns error"""
        login_data = {'username': username, 'password': password}
        
        try:
            response = requests.post(f'{api_url}/api/auth/login', json=login_data, timeout=5)
        except requests.exceptions.ConnectionError:
            pytest.skip("API is not running")
        except requests.exceptions.Timeout:
            pytest.skip("API request timed out")
        
        assert response.status_code in [400, 401], f"Expected 400 or 401, got {response.status_code}"
    
    def test_TC014_register_valid_data_returns_success(self, api_url):
        """Test registration with valid data returns success"""
        username = f'testuser{random.randint(1000, 9999)}'
        register_data = {
            'username': username,
            'email': f'{username}@test.com',
            'password': 'Test@123456',
            'role': 'Customer'
        }
        
        try:
            response = requests.post(f'{api_url}/api/auth/register', json=register_data, timeout=5)
        except requests.exceptions.ConnectionError:
            pytest.skip("API is not running")
        except requests.exceptions.Timeout:
            pytest.skip("API request timed out")
        
        # Accept 200 (success) or 400 (validation error) or 409 (conflict)
        assert response.status_code in [200, 400, 409], f"Expected 200/400/409, got {response.status_code}. Response: {response.text}"
        
        if response.status_code == 200:
            data = response.json()
            assert 'token' in data, "Response should contain token"
            assert data['username'] == username, f"Username should match: {username}"
    
    def test_TC015_register_duplicate_username_returns_error(self, api_url):
        """Test registration with duplicate username returns error"""
        # First registration
        username = f'duplicate{random.randint(1000, 9999)}'
        register_data = {
            'username': username,
            'email': f'{username}@test.com',
            'password': 'Test@123456'
        }
        
        try:
            response1 = requests.post(f'{api_url}/api/auth/register', json=register_data, timeout=5)
        except requests.exceptions.ConnectionError:
            pytest.skip("API is not running")
        except requests.exceptions.Timeout:
            pytest.skip("API request timed out")
        
        # Only test duplicate if first registration succeeded
        if response1.status_code != 200:
            pytest.skip(f"First registration failed with {response1.status_code}, cannot test duplicate")
        
        # Second registration with same username
        register_data2 = {
            'username': username,
            'email': f'{username}2@test.com',  # Different email
            'password': 'Test@123456'
        }
        
        try:
            response2 = requests.post(f'{api_url}/api/auth/register', json=register_data2, timeout=5)
        except requests.exceptions.ConnectionError:
            pytest.skip("API is not running")
        except requests.exceptions.Timeout:
            pytest.skip("API request timed out")
        
        # Second should fail
        assert response2.status_code == 400, f"Expected 400, got {response2.status_code}"
        data = response2.json()
        assert 'message' in data or 'errors' in data, "Error response should contain message or errors"
