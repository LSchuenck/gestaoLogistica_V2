import { defineStore } from "pinia"
import { ref } from "vue"
import { extrairMensagemErro } from "@/plugin/axios"

// Controla a mensagem de feedback (v-snackbar) mostrada após cada operação
export const useMensagemStore = defineStore("mensagem", () => {
    const visivel = ref(false);
    const texto = ref("");
    const cor = ref("success");
    const tempo = ref(4000);

    const mostrarSucesso = (mensagem) => {
        texto.value = mensagem;
        cor.value = "success";
        tempo.value = 4000;
        visivel.value = true;
    }

    const mostrarErro = (error) => {
        texto.value = typeof error === "string" ? error : extrairMensagemErro(error);
        cor.value = "error";
        tempo.value = 4000;
        visivel.value = true;
    }

    // Aviso que fica na tela até ser fechado (tempo -1)
    const mostrarAviso = (mensagem) => {
        texto.value = mensagem;
        cor.value = "warning";
        tempo.value = -1;
        visivel.value = true;
    }

    return { visivel, texto, cor, tempo, mostrarSucesso, mostrarErro, mostrarAviso }
});
