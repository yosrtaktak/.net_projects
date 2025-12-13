# -*- coding: utf-8 -*-
import pytest
import requests

@pytest.mark.api
@pytest.mark.vehicles
@pytest.mark.integration
class TestVehiclesAPI:
    
    def test_TC018_get_all_vehicles(self, api_url):
        """Test GET /api/vehicles returns success or requires auth"""
        try:
            response = requests.get(f'{api_url}/api/vehicles', timeout=5)
        except requests.exceptions.ConnectionError:
            pytest.skip("API is not running. Start Backend with 'dotnet run' in Backend folder")
        except requests.exceptions.Timeout:
            pytest.skip("API request timed out")
        
        # API might require authentication (401) or return success (200)
        assert response.status_code in [200, 401], f"Expected 200 or 401, got {response.status_code}"
        
        if response.status_code == 200:
            data = response.json()
            assert isinstance(data, list), "Response should be a list of vehicles"
    
    def test_TC019_get_vehicle_by_id_existing(self, api_url):
        """Test GET /api/vehicles/{id} with existing ID"""
        try:
            response = requests.get(f'{api_url}/api/vehicles/1', timeout=5)
        except requests.exceptions.ConnectionError:
            pytest.skip("API is not running")
        except requests.exceptions.Timeout:
            pytest.skip("API request timed out")
        
        # Could be 200 (found), 401 (auth required), or 404 (not found if DB empty)
        assert response.status_code in [200, 401, 404], f"Expected 200/401/404, got {response.status_code}"
        
        if response.status_code == 200:
            data = response.json()
            assert 'id' in data or 'Id' in data, "Vehicle should have an id field"
    
    def test_TC020_get_vehicle_by_id_nonexisting(self, api_url):
        """Test GET /api/vehicles/{id} with non-existing ID returns 404 or 401"""
        try:
            response = requests.get(f'{api_url}/api/vehicles/99999', timeout=5)
        except requests.exceptions.ConnectionError:
            pytest.skip("API is not running")
        except requests.exceptions.Timeout:
            pytest.skip("API request timed out")
        
        # Should be 404 (not found) or 401 (auth required)
        assert response.status_code in [401, 404], f"Expected 401 or 404, got {response.status_code}"
    
    def test_TC021_get_vehicles_with_auth_token(self, api_url, auth_token):
        """Test GET /api/vehicles with authentication token"""
        if not auth_token:
            pytest.skip("Could not obtain auth token. Check if admin user exists")
        
        headers = {
            'Authorization': f'Bearer {auth_token}',
            'Content-Type': 'application/json'
        }
        
        try:
            response = requests.get(f'{api_url}/api/vehicles', headers=headers, timeout=5)
        except requests.exceptions.ConnectionError:
            pytest.skip("API is not running")
        except requests.exceptions.Timeout:
            pytest.skip("API request timed out")
        
        assert response.status_code == 200, f"Expected 200 with valid token, got {response.status_code}"
        data = response.json()
        assert isinstance(data, list), "Response should be a list"
    
    def test_TC022_search_vehicles_by_query(self, api_url):
        """Test vehicle search functionality"""
        try:
            # Try searching for vehicles with query parameter
            response = requests.get(f'{api_url}/api/vehicles?search=car', timeout=5)
        except requests.exceptions.ConnectionError:
            pytest.skip("API is not running")
        except requests.exceptions.Timeout:
            pytest.skip("API request timed out")
        
        # Should return 200 or 401
        assert response.status_code in [200, 401], f"Expected 200 or 401, got {response.status_code}"
