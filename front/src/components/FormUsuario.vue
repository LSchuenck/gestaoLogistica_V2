<script setup>
    import { ref } from 'vue';
    import { useUsuarioStore } from '@/stores/usuario';
    import { useMensagemStore } from '@/stores/mensagem';
    import { obrigatorio, emailValido, senhaValida, copiarObjeto } from '@/utils/formulario';

    const props = defineProps(['usuario']);
    const emit = defineEmits(['fechar']);

    const storeUsuario = useUsuarioStore();
    const mensagemStore = useMensagemStore();

    const perfis = ['ADMIN', 'GERENTE', 'OPERADOR'];

    const formulario = ref(null);
    const salvando = ref(false);
    const item = ref(props.usuario ? { ...copiarObjeto(props.usuario), senha: '' } : {
        id: 0,
        nome: '',
        email: '',
        perfil: 'OPERADOR'
    });

    const ehEdicao = !!item.value.id;

    // Na edição a senha é opcional: vazia mantém a atual
    const senhaOpcional = (valor) => !valor || senhaValida(valor);

    const salvar = async () => {
        const { valid } = await formulario.value.validate();
        if (!valid) return;

        salvando.value = true;
        try {
            const resposta = ehEdicao
                ? await storeUsuario.alterarUsuario(item.value)
                : await storeUsuario.cadastrarUsuario(item.value);
            // Se o e-mail falhou, a mensagem traz a senha temporária e precisa ficar na tela
            if (resposta.dados.senhaTemporaria) {
                mensagemStore.mostrarAviso(resposta.mensagem);
            } else {
                mensagemStore.mostrarSucesso(resposta.mensagem);
            }
            emit('fechar');
        } catch (error) {
            mensagemStore.mostrarErro(error);
        } finally {
            salvando.value = false;
        }
    }
</script>

<template>
    <v-card>
        <v-card-title class="text-h6">
            {{ ehEdicao ? 'Editar usuário' : 'Novo usuário' }}
        </v-card-title>
        <v-card-text>
            <v-form ref="formulario" @submit.prevent="salvar">
                <v-text-field v-model="item.nome" label="Nome *" :rules="[obrigatorio]" density="compact"></v-text-field>
                <v-text-field v-model="item.email" label="E-mail *" :rules="[obrigatorio, emailValido]" density="compact"></v-text-field>
                <v-select v-model="item.perfil" :items="perfis" label="Perfil *" :rules="[obrigatorio]" density="compact"></v-select>
                <v-text-field v-if="ehEdicao" v-model="item.senha" type="password" :rules="[senhaOpcional]" density="compact"
                    label="Nova senha (deixe em branco para manter)"></v-text-field>
                <div v-else class="text-caption mb-2">O usuário recebe uma senha temporária por e-mail e cria a própria senha no primeiro acesso.</div>

                <div class="d-flex justify-end mt-2">
                    <v-btn type="button" class="mr-2" @click="emit('fechar')">Cancelar</v-btn>
                    <v-btn type="submit" color="success" prepend-icon="mdi-content-save" :loading="salvando">Salvar</v-btn>
                </div>
            </v-form>
        </v-card-text>
    </v-card>
</template>
