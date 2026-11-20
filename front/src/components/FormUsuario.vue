<script setup>
    import { ref } from 'vue';
    import { useUsuarioStore } from '@/stores/usuario';
    import { useMensagemStore } from '@/stores/mensagem';
    import { obrigatorio, emailValido, copiarObjeto } from '@/utils/formulario';

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
        perfil: 'OPERADOR',
        senha: ''
    });

    const ehEdicao = !!item.value.id;

    // No cadastro a senha é obrigatória; na edição só se quiser trocar
    const regraSenha = (valor) => {
        if (!valor) return ehEdicao || 'Campo obrigatório';
        return valor.length >= 6 || 'A senha deve ter no mínimo 6 caracteres';
    }

    const salvar = async () => {
        const { valid } = await formulario.value.validate();
        if (!valid) return;

        salvando.value = true;
        try {
            const resposta = ehEdicao
                ? await storeUsuario.alterarUsuario(item.value)
                : await storeUsuario.cadastrarUsuario(item.value);
            mensagemStore.mostrarSucesso(resposta.mensagem);
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
                <v-text-field v-model="item.senha" type="password" :rules="[regraSenha]" density="compact"
                    :label="ehEdicao ? 'Nova senha (deixe em branco para manter)' : 'Senha *'"></v-text-field>

                <div class="d-flex justify-end mt-2">
                    <v-btn type="button" class="mr-2" @click="emit('fechar')">Cancelar</v-btn>
                    <v-btn type="submit" color="success" prepend-icon="mdi-content-save" :loading="salvando">Salvar</v-btn>
                </div>
            </v-form>
        </v-card-text>
    </v-card>
</template>
