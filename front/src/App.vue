<script setup>
import { RouterView, useRoute, useRouter } from 'vue-router'
import { computed, ref } from 'vue';
import { useAuthStore } from '@/stores/auth';
import { useMensagemStore } from '@/stores/mensagem';
import { storeToRefs } from 'pinia';

const route = useRoute();
const router = useRouter();
const authStore = useAuthStore();
const mensagemStore = useMensagemStore();
const { visivel, texto, cor, tempo } = storeToRefs(mensagemStore);

// null: o Vuetify deixa o menu aberto no computador e fechado no celular
const drawer = ref(null);

// O menu só aparece fora da tela de login
const mostrarMenu = computed(() => route.path !== '/' && route.path !== '/trocar-senha');

const itensMenu = computed(() => {
  const cadastros = [
    { titulo: 'Transportadoras', rota: '/transportadoras', icone: 'mdi-domain' },
    { titulo: 'Motoristas', rota: '/motoristas', icone: 'mdi-card-account-details' },
    { titulo: 'Veículos', rota: '/veiculos', icone: 'mdi-truck' },
    { titulo: 'Clientes', rota: '/clientes', icone: 'mdi-account-group' },
    { titulo: 'Armazéns', rota: '/armazens', icone: 'mdi-warehouse' },
    { titulo: 'Produtos', rota: '/produtos', icone: 'mdi-package-variant-closed' },
  ];
  const estoque = { titulo: 'Estoque', rota: '/estoque', icone: 'mdi-swap-vertical' };
  const usuarios = { titulo: 'Usuários', rota: '/usuarios', icone: 'mdi-account-cog' };

  // Operador: só estoque. Gerente: cadastros e estoque. Administrador: tudo.
  if (authStore.ehOperador) return [estoque];
  if (authStore.ehAdministrador) return [...cadastros, estoque, usuarios];
  return [...cadastros, estoque];
});

const sair = () => {
  authStore.sair();
  mensagemStore.mostrarSucesso('Você saiu do sistema.');
  router.push('/');
}
</script>

<template>
  <v-app>
    <v-app-bar title="Gestão Logística" color="primary">
      <template v-slot:prepend>
        <v-app-bar-nav-icon @click="drawer = !drawer" v-if="mostrarMenu" />
      </template>
      <template v-slot:append v-if="mostrarMenu">
        <span class="mr-4 d-none d-sm-inline">
          <v-icon icon="mdi-account" size="small" class="mr-1"></v-icon>
          {{ authStore.usuario?.nome }} ({{ authStore.usuario?.perfil }})
        </span>
      </template>
    </v-app-bar>

    <v-navigation-drawer v-model="drawer" v-if="mostrarMenu">
      <v-list nav density="compact">
        <v-list-item v-for="item in itensMenu" :key="item.rota" :to="item.rota"
          :title="item.titulo" :prepend-icon="item.icone" color="primary"></v-list-item>
      </v-list>
      <template v-slot:append>
        <div class="pa-2">
          <v-btn block color="primary" prepend-icon="mdi-logout" @click="sair">
            Sair
          </v-btn>
        </div>
      </template>
    </v-navigation-drawer>

    <v-main>
      <v-container fluid>
        <RouterView />
      </v-container>
    </v-main>

    <!-- Feedback de todas as operações -->
    <v-snackbar v-model="visivel" :color="cor" :timeout="tempo" location="top right">
      {{ texto }}
      <template v-slot:actions>
        <v-btn variant="text" icon="mdi-close" @click="visivel = false"></v-btn>
      </template>
    </v-snackbar>
  </v-app>
</template>
