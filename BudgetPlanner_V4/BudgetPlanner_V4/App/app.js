var app = angular.module('budget_planner', ['ui.router', 'ui.bootstrap', 'LocalStorageModule', 'uiSwitch', 'trNgGrid', 'nvd3', 'angular-loading-bar']);

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
                currentmonth: function (dashboardsvc) {
                    console.log('resolving')
                    return dashboardsvc.dates();
                },
                currentvalues: function (dashboardsvc) {
                    return dashboardsvc.cvalues();
                },
                yearlyvalues: function (dashboardsvc) {
                    return dashboardsvc.yvalues();
                },
                account: function (houseaccountsvc) {
                    return houseaccountsvc.list();
                },
                transactions: function (transactionsvc) {
                    return transactionsvc.recent();
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
          controller: 'householdCtrl as house'
      })
      .state('household.details', {
          url: "",
          templateUrl: "/app/templates/household/household.details.html",
          resolve: {
              household: function (houseSvc) {
                  return houseSvc.details();
              }
          },
          data: {
              requiresHousehold : true
          },
          controller: "houseDetailsCtrl as houseDetails",
      })

////=================================================================================//

       //HOUSEHOLD BEGIN
        .state('household_begin', {
            url: "/household_begin",
            templateUrl: "/app/templates/household/household.begin.html",
            controller: "householdBeginCtrl as houseBegin",
        })

////=================================================================================//

//      //ACCOUNT STATES
//      .state('accounts', {
//          url: "/accounts",
//          templateUrl: "/app/templates/accounts/accounts.html",
//          abstract:true,
//          controller: "accountCtrl as account"
//      })
//      .state('accounts.list', {
//          url: "",
//          templateUrl: "/app/templates/accounts/accounts.list.html",
//          resolve: {
//              account: function (houseAccountSvc) {
//                return houseAccountSvc.list();
//              }
//          },
//          controller: "accountListCtrl as accountList",
//          data: {
//              requiresHousehold : true
//          }
//      })
//      .state('accounts.details', {
//          url: "/details/:id",
//          templateUrl: "/app/templates/accounts/accounts.details.html",
//          controller: "accountDetailsCtrl as accountDetails",
//          resolve: {
//              account: ['houseAccountSvc', '$stateParams', function (houseAccountSvc, $stateParams) {
//                  console.log($stateParams)
//                  return houseAccountSvc.details($stateParams.id)
//              }],
//              categories: function (categorySvc) {
//                  return categorySvc.list();
//              },

//          }
//      })

////=================================================================================//

//     //BUDGET STATES
//      .state('budget', {
//          url: "/budget",
//          templateUrl: "/app/templates/budget/budget.html",
//          abstract: true,
//          controller: "budgetCtrl as budget"
//      })

//      .state('budget.list', {
//          url: "",
//          templateUrl: "/app/templates/budget/budget.list.html",
//          resolve: {
//              budget: function (budgetItemSvc) {
//                  return budgetItemSvc.list();
//              },
//              categories: function (categorySvc) {
//                  return categorySvc.list();
//              }
//          },
//          data: {
//              requiresHousehold : true
//          },
//          controller: "budgetListCtrl as budgetList"
//      })

      //.state('budget.categories', {
      //    url: "/categories",
      //    templateUrl: "/app/templates/budget/budget.categories.html",
      //    resolve: {
      //        category: function (categorySvc) {
      //            return categorySvc.list();
      //        }
      //    },
      //    controller: "budgetCategoryCtrl as category"
      //})
     


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
                event.preventDefault();
                $state.go('login');
            }
            if (authService.authentication.householdId == null ||
                authService.authentication.householdId == "") {
                event.preventDefault();
                console.log('YOU SHALL NOT PASS')
                $state.go('household_begin');
            }

        }
    });

}]);