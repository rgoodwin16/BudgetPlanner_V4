﻿var app = angular.module('budget_planner', ['ui.router', 'ui.bootstrap', 'LocalStorageModule', 'uiSwitch', 'trNgGrid', 'nvd3', 'angular-loading-bar']);

app.config(function ($stateProvider, $urlRouterProvider) {
    //
    // For any unmatched url, redirect to /state1
    $urlRouterProvider.otherwise("/login");
    //
    // Now set up the states
    $stateProvider

        //LOGIN STATES
      .state('login', {
          url: "/login",
          templateUrl: "/app/templates/login/login.html",
          abstract:true,
          controller: "loginCtrl as user"
      })

      .state('login.signin', {
          params: {'isNew' : false},
          url: "",
          templateUrl: "/app/templates/login/login.signin.html",
          controller: "loginCtrl as user"
      })

      .state('login.register', {
          url: "/register",
          templateUrl: "/app/templates/login/login.register.html",
          controller: "registerCtrl as register"
      })

//      .state('login.forgot', {
//          url: "/forgot_password",
//          templateUrl: "/app/templates/login/login.forgot.html",
//          controller: "forgotCtrl as forgot"
//      })


////=================================================================================//

//        //USER MANAGE STATES
//        .state('user_manage', {
//            url: "/user_profile",
//            templateUrl: "/app/templates/user/user.manage.html",
//            controller: "userManageCtrl as user"

//        })

////=================================================================================//

//        //LANDING PAGE STATES
//        .state('home', {
//            url: "/home",
//            templateUrl: "/app/templates/home/home.html",
//            controller: "homeCtrl as home"
//        })

////=================================================================================//

       //DASHBOARD STATES
        .state('dashboard', {
            url: "/dashboard",
            templateurl: "/app/templates/dashboard/dashboard.html",
            controller: "dashboardCtrl as dashboard",
            resolve: {
                currentMonth: function (dashboardSvc) {
                    return dashboardSvc.dates();
                },
                currentValues: function (dashboardSvc) {
                    return dashboardSvc.cvalues();
                },
                yearlyValues: function (dashboardSvc) {
                    return dashboardSvc.yvalues();
                },
                account: function (householdAccountSvc) {
                    return householdAccountSvc.list();
                },
                transactions: function (transactionSvc) {
                    return transactionSvc.recent();
                }
            },
            data: {
                requireshousehold: true
            }
            
        })
////=================================================================================//

       //HOUSEHOLD STATES
      .state('household', {
          url: "/household",
          templateUrl: "/app/templates/household/household.html",
          abstract: true,
      })
      .state('household.details', {
          url: "",
          templateUrl: "/app/templates/household/household.details.html",
          resolve: {
              household: function (householdSvc) {
                  return householdSvc.details();
              }
          },
          data: {
              requiresHousehold : true
          },
          controller: "householdDetailsCtrl as householdDetails",
      })

////=================================================================================//

       //HOUSEHOLD BEGIN
        .state('household_begin', {
            url: "/household_begin",
            templateUrl: "/app/templates/household/household.begin.html",
            controller: "householdBeginCtrl as householdBegin",
        })

////=================================================================================//

      //ACCOUNT STATES
      .state('accounts', {
          url: "/accounts",
          templateUrl: "/app/templates/household_account/account.html",
          abstract:true,
      })
      .state('accounts.list', {
          url: "",
          templateUrl: "/app/templates/household_account/account.list.html",
          resolve: {
              account: function (householdAccountSvc) {
                  return householdAccountSvc.list();
              }
          },
          controller: "householdAccountListCtrl as accountList",
          data: {
              requiresHousehold : true
          }
      })
      .state('accounts.details', {
          url: "/details/:id",
          templateUrl: "/app/templates/household_account/account.details.html",
          controller: "householdAccountDetailsCtrl as accountDetails",
          resolve: {
              account: ['householdAccountSvc', '$stateParams', function (householdAccountSvc, $stateParams) {
                  console.log($stateParams)
                  return householdAccountSvc.details($stateParams.id)
              }],
              categories: function (categorySvc) {
                  return categorySvc.list();
              },

          }
      })

////=================================================================================//

     //BUDGET STATES
      .state('budget', {
          url: "/budget",
          templateUrl: "/app/templates/budget_item/budget.html",
          abstract: true,
          //controller: "budgetCtrl as budget"
      })

      .state('budget.list', {
          url: "",
          templateUrl: "/app/templates/budget_item/budget.list.html",
          resolve: {
              budget: function (budgetItemSvc) {
                  return budgetItemSvc.list();
              },
              categories: function (categorySvc) {
                  return categorySvc.list();
              }
          },
          data: {
              requiresHousehold : true
          },
          controller: "budgetItemListCtrl as budgetList"
      })

});

//var serviceBase = 'http://rgoodwin-budget.azurewebsites.net/';
var serviceBase = 'http://localhost:51187/';

app.constant('ngAuthSettings', {
    apiServiceBaseUri: serviceBase
});

app.config(function ($httpProvider) {
    $httpProvider.interceptors.push('authInterceptorSvc');
});

app.run(['$rootScope','$state','$stateParams','authSvc', function ($rootScope,$state,$stateParams,authService) {
    $rootScope.$state = $state;
    $rootScope.$state.$stateParams = $stateParams;
    authService.fillAuthData();

    $rootScope.$on('$stateChangeStart', function (event, toState, toParams, fromParams) {

        console.log('state change');

        if (toState.data && toState.data.requiresHousehold === true) {
            if (!authService.authentication.isAuth) {
                //event.preventDefault();
                $state.go('login');
                console.log('problem')
            }
            if (authService.authentication.householdId == null ||
                authService.authentication.householdId == "") {
                //event.preventDefault();
                console.log('YOU SHALL NOT PASS')
                $state.go('household_begin');
            }

        }
    });

}]);