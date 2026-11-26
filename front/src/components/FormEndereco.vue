<script setup>
    import { ref } from 'vue';
    import { estados, mascaraCep, apenasNumeros } from '@/utils/formatacao';
    import { aplicarMascara } from '@/utils/formulario';

    // Campos de endereço usados nos formulários de transportadora, cliente e armazém
    const endereco = defineModel({ type: Object });

    const campoNumero = ref(null);
    const buscandoCep = ref(false);

    // Ao digitar o CEP: aplica a máscara e, com 8 dígitos, busca o endereço no ViaCEP
    const alterarCep = async () => {
        aplicarMascara(endereco.value, 'cep', mascaraCep);

        const cep = apenasNumeros(endereco.value.cep);
        if (cep.length !== 8) return;

        buscandoCep.value = true;
        try {
            const resposta = await fetch('https://viacep.com.br/ws/' + cep + '/json/');
            const dados = await resposta.json();
            if (dados.erro) return; // CEP não encontrado: deixa os campos como estão

            endereco.value.logradouro = dados.logradouro;
            endereco.value.bairro = dados.bairro;
            endereco.value.cidade = dados.localidade;
            endereco.value.estado = dados.uf;
            campoNumero.value.focus();
        } catch {
            // Sem internet ou ViaCEP fora do ar: a pessoa preenche à mão
        } finally {
            buscandoCep.value = false;
        }
    }
</script>

<template>
    <div class="text-subtitle-2 mb-2">Endereço</div>
    <v-row dense>
        <v-col cols="12" sm="4">
            <v-text-field v-model="endereco.cep" @update:model-value="alterarCep" maxlength="9" label="CEP" :loading="buscandoCep" density="compact"></v-text-field>
        </v-col>
        <v-col cols="12" sm="8">
            <v-text-field v-model="endereco.logradouro" label="Logradouro" density="compact"></v-text-field>
        </v-col>
        <v-col cols="12" sm="4">
            <v-text-field ref="campoNumero" v-model="endereco.numero" label="Número" density="compact"></v-text-field>
        </v-col>
        <v-col cols="12" sm="8">
            <v-text-field v-model="endereco.complemento" label="Complemento" density="compact"></v-text-field>
        </v-col>
        <v-col cols="12" sm="5">
            <v-text-field v-model="endereco.bairro" label="Bairro" density="compact"></v-text-field>
        </v-col>
        <v-col cols="12" sm="5">
            <v-text-field v-model="endereco.cidade" label="Cidade" density="compact"></v-text-field>
        </v-col>
        <v-col cols="12" sm="2">
            <v-select v-model="endereco.estado" :items="estados" label="UF" density="compact"></v-select>
        </v-col>
    </v-row>
</template>
