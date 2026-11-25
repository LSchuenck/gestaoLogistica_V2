import { createRouter, createWebHistory } from 'vue-router'
import Login from '../views/Login.vue'
import { useAuthStore } from '@/stores/auth'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'login',
      component: Login,
      meta: { publica: true }
    },
    {
      path: '/transportadoras',
      name: 'transportadoras',
      component: () => import("@/views/Transportadoras.vue")
    },
    {
      path: '/motoristas',
      name: 'motoristas',
      component: () => import("@/views/Motoristas.vue")
    },
    {
      path: '/veiculos',
      name: 'veiculos',
      component: () => import("@/views/Veiculos.vue")
    },
    {
      path: '/clientes',
      name: 'clientes',
      component: () => import("@/views/Clientes.vue")
    },
    {
      path: '/armazens',
      name: 'armazens',
      component: () => import("@/views/Armazens.vue")
    },
    {
      path: '/produtos',
      name: 'produtos',
      component: () => import("@/views/Produtos.vue")
    },
    {
      path: '/estoque',
      name: 'estoque',
      component: () => import("@/views/Estoque.vue")
    },
    {
      path: '/usuarios',
      name: 'usuarios',
      component: () => import("@/views/Usuarios.vue"),
      meta: { somenteAdministrador: true }
    },
    // Endereço que não existe volta para o início
    {
      path: '/:caminho(.*)*',
      redirect: '/'
    }
  ],
})

// Protege as páginas: sem login volta para o login; tela de usuários só para ADMIN
router.beforeEach((destino) => {
  const auth = useAuthStore();

  if (destino.meta.publica) {
    return auth.estaLogado ? '/transportadoras' : true;
  }

  if (!auth.estaLogado) {
    return '/';
  }

  if (destino.meta.somenteAdministrador && !auth.ehAdministrador) {
    return '/transportadoras';
  }

  return true;
})

export default router
