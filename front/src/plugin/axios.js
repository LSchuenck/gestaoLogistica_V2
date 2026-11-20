import axios from "axios";

// Endereço da API vem do arquivo .env (VITE_API_URL)
const api = axios.create({
    baseURL: import.meta.env.VITE_API_URL
});

// Antes de cada requisição: envia o token JWT no cabeçalho Authorization
api.interceptors.request.use(
    (config) => {
        const token = localStorage.getItem('token');

        if (token) {
            config.headers.Authorization = `Bearer ${token}`
        }

        return config
    },
    (error) => {
        return Promise.reject(error)
    }
)

// Depois de cada resposta: se o token expirou (401), limpa o login e volta para a tela de login
api.interceptors.response.use(
    (response) => response,
    (error) => {
        const ehTelaDeLogin = error.config?.url?.includes('/autenticacao/login');

        if (error.response?.status === 401 && !ehTelaDeLogin) {
            localStorage.removeItem('token');
            localStorage.removeItem('usuario');
            window.location.href = '/?sessaoExpirada=1';
        }

        return Promise.reject(error)
    }
)

// Monta o texto de erro a partir da resposta padronizada da API (ResultWrapper)
export function extrairMensagemErro(error) {
    const resposta = error.response?.data;

    if (!resposta) {
        return 'Não foi possível conectar à API. Verifique se ela está em execução.';
    }

    const erros = resposta.erros ?? [];
    // Se a lista de erros só repete a mensagem, mostra apenas a mensagem
    if (erros.length === 0 || (erros.length === 1 && erros[0] === resposta.mensagem)) {
        return resposta.mensagem;
    }

    return `${resposta.mensagem} ${erros.join(' ')}`;
}

export default api;
