import { createRouter, createWebHistory } from 'vue-router'
import Login from '../views/Login.vue'
import { useAuthStore } from '@/stores/auth'
import { useMensagemStore } from '@/stores/mensagem'

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
      component: () => import("@/views/Transportadoras.vue"),
      meta: { perfis: ['ADMIN', 'GERENTE'] }
    },
    {
      path: '/motoristas',
      name: 'motoristas',
      component: () => import("@/views/Motoristas.vue"),
      meta: { perfis: ['ADMIN', 'GERENTE'] }
    },
    {
      path: '/veiculos',
      name: 'veiculos',
      component: () => import("@/views/Veiculos.vue"),
      meta: { perfis: ['ADMIN', 'GERENTE'] }
    },
    {
      path: '/clientes',
      name: 'clientes',
      component: () => import("@/views/Clientes.vue"),
      meta: { perfis: ['ADMIN', 'GERENTE'] }
    },
    {
      path: '/armazens',
      name: 'armazens',
      component: () => import("@/views/Armazens.vue"),
      meta: { perfis: ['ADMIN', 'GERENTE'] }
    },
    {
      path: '/produtos',
      name: 'produtos',
      component: () => import("@/views/Produtos.vue"),
      meta: { perfis: ['ADMIN', 'GERENTE'] }
    },
    {
      path: '/estoque',
      name: 'estoque',
      component: () => import("@/views/Estoque.vue")
    },
    {
      path: '/viagens',
      name: 'viagens',
      component: () => import("@/views/Viagens.vue")
    },
    {
      path: '/trocar-senha',
      name: 'trocar-senha',
      component: () => import("@/views/TrocarSenha.vue")
    },
    {
      path: '/usuarios',
      name: 'usuarios',
      component: () => import("@/views/Usuarios.vue"),
      meta: { perfis: ['ADMIN'] }
    },
    // Endereço que não existe volta para o início
    {
      path: '/:caminho(.*)*',
      redirect: '/'
    }
  ],
})

// Protege as páginas: exige login, a troca da senha temporária e o perfil certo
router.beforeEach((destino) => {
  const auth = useAuthStore();

  if (destino.meta.publica) {
    return auth.estaLogado ? auth.paginaInicial : true;
  }

  if (!auth.estaLogado) {
    return '/';
  }

  // Primeiro acesso: só pode ficar na página de trocar a senha
  if (auth.precisaTrocarSenha !== (destino.path === '/trocar-senha')) {
    return auth.paginaInicial;
  }

  if (destino.meta.perfis && !destino.meta.perfis.includes(auth.usuario.perfil)) {
    useMensagemStore().mostrarErro('Seu perfil não tem acesso a esta página.');
    return auth.paginaInicial;
  }

  return true;
})

export default router
