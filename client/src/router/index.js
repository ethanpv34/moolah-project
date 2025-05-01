import { createRouter, createWebHistory } from 'vue-router';
import Login from '../components/Login.vue';
import TodoList from '../components/TodoList.vue';

const routes = [
  {
    path: '/',
    redirect: '/todos'
  },
  {
    path: '/login',
    name: 'Login',
    component: Login
  },
  {
    path: '/todos',
    name: 'Todos',
    component: TodoList,
    meta: { requiresAuth: true }
  }
];

const router = createRouter({
  history: createWebHistory(),
  routes
});

router.beforeEach((to, from, next) => {
  const isAuthenticated = !!localStorage.getItem('user');
  
  if (to.meta.requiresAuth && !isAuthenticated) {
    next('/login');
  } else {
    next();
  }
});

export default router; 