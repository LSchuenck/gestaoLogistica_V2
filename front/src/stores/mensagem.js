import { defineStore } from "pinia"
import { ref } from "vue"
import { extrairMensagemErro } from "@/plugin/axios"

// Controla a mensagem de feedback (v-snackbar) mostrada após cada operação
export const useMensagemStore = defineStore("mensagem", () => {
    const visivel = ref(false);
    const texto = ref("");
    const cor = ref("success");

    const mostrarSucesso = (mensagem) => {
        texto.value = mensagem;
        cor.value = "success";
        visivel.value = true;
    }

    const mostrarErro = (error) => {
        texto.value = typeof error === "string" ? error : extrairMensagemErro(error);
        cor.value = "error";
        visivel.value = true;
    }

    return { visivel, texto, cor, mostrarSucesso, mostrarErro }
});
