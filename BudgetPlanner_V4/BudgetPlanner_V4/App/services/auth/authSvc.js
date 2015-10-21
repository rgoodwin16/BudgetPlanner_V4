'use strict';
angular.module('budget_planner')
.factory('authSvc', ['$http', '$q', 'localStorageService', 'ngAuthSettings', function ($http, $q, localStorageService, ngAuthSettings) {

    var serviceBase = ngAuthSettings.apiServiceBaseUri;
    var authServiceFactory = {};

    var _authentication = {
        isAuth: false,
        userName: "",
        householdId: null
    };

    var _saveRegistration = function (registration) {

        _logout();

        return $http.post(serviceBase + 'api/account/register', registration).then(function (response) {
            return response;
        });

    };

    var _register = function (model) {
        return $http.post(serviceBase + 'api/account/register', model).then(function (response) {
            return response;
        });
    };

    var _login = function (username, password) {

        var data = "grant_type=password&username=" + username + "&password=" + password;

        var deferred = $q.defer();

        $http.post(serviceBase + 'token', data, { headers: { 'Content-Type': 'application/x-www-form-urlencoded' } }).success(function (response) {

            localStorageService.set('authorizationData', {
                token: response.access_token, username: username, refreshToken: response.refresh_token,
                householdId: response.householdId
            });

            _authentication.isAuth = true;
            _authentication.username = username;
            _authentication.householdId = response.householdId;

            deferred.resolve(response);

        }).error(function (err, status) {
            _logout();
            deferred.reject(err);
        });

        return deferred.promise;

    };

    var _logout = function () {

        localStorageService.remove('authorizationData');

        _authentication.isAuth = false;
        _authentication.username = "";
        _authentication.householdId = null

    };

    var _fillAuthData = function () {

        var authData = localStorageService.get('authorizationData');
        if (authData) {
            _authentication.isAuth = true;
            _authentication.username = authData.username;
            _authentication.householdId = authData.householdId
        }

    };

    var _refreshToken = function () {
        var deferred = $q.defer();

        var authData = localStorageService.get('authorizationData');

        if (authData) {

            var data = "grant_type=refresh_token&refresh_token=" + authData.refreshToken;

            localStorageService.remove('authorizationData');

            $http.post(serviceBase + 'token', data, { headers: { 'Content-Type': 'application/x-www-form-urlencoded' } }).success(function (response) {

                localStorageService.set('authorizationData', {
                    token: response.access_token, username: response.username, refreshToken: response.refresh_token,
                householdId: response.householdId});

                deferred.resolve(response);

            }).error(function (err, status) {
                _logout();
                deferred.reject(err);
            });
        }

        return deferred.promise;
    };

    authServiceFactory.saveRegistration = _saveRegistration;
    authServiceFactory.login = _login;
    authServiceFactory.logout = _logout;
    authServiceFactory.fillAuthData = _fillAuthData;
    authServiceFactory.authentication = _authentication;
    authServiceFactory.refresh = _refreshToken;
    authServiceFactory.register = _register;

    return authServiceFactory;
}]);