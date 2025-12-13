# Simple script to create test files
import os

test_auth = '''import pytest
import requests

@pytest.mark.api
class TestAuth:
    def test_TC011_login_valid(self, api_url):
        response = requests.post(f'{api_url}/api/auth/login', json={
            'email': 'admin@carrental.com',
            'password': 'Admin@123'
        })
        assert response.status_code == 200
        data = response.json()
        assert 'token' in data
'''

os.makedirs('tests', exist_ok=True)
with open('tests/test_auth_api.py', 'w') as f:
    f.write(test_auth)

print("Test files created!")
