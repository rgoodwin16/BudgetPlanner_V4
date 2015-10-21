'use strict';
angular.module('budget_planner').controller('layoutCtrl', ['$state','authSvc', function ($state,authSvc) {
    var self = this;

    self.$state = $state;

    this.logout = function () {
        authSvc.logout();
        $state.go('login.signin');
    }

}])