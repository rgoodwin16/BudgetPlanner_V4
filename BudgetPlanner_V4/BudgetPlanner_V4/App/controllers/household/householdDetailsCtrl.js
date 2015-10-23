'use strict';
angular.module('budget_planner').controller('householdDetailsCtrl', ['authSvc', 'householdSvc', '$state', 'household', 'validationSvc', function (authSvc, householdSvc, $state, household, validationSvc) {
    var self = this;

    this.household = household;
    this.inviteEmail = '';
    this.inviteToggle = 0;
    this.panel = 'm';
    this.errors = null;

    //CLEAR FORM
    this.clear = function () {
        $state.go($state.current, null, { reload: true })
    }

    //BEGIN INVITE
    this.beginInvite = function () {
        self.panel = 'i';
    }

    //CREATE INVITE 
    this.createInvite = function () {
        householdSvc.invite(self.inviteEmail)
        self.inviteToggle = 1;
        self.panel = 'm';
    }

    //VALIDATE EMAIL FORM
    this.isValidEmail = function () {
        return validationSvc.isEmail(self.inviteEmail);
    }


    //LEAVE HOUSEHOLD - SEND CONFIRM MESSAGE
    this.confirmLeave = function () {
        self.panel = 'l';
    }

    //LEAVE HOUSEHOLD - CONFRIM LEAVE
    this.leaveHousehold = function () {
        householdSvc.leave().then(function (success) {
            authSvc.refresh().then(function (success) {
                $state.go('household_begin');
            })
        })
    }

}])