// Funções simples para exibir dados no formato brasileiro

// "2027-08-15" -> "15/08/2027"
export function formatarData(data) {
    if (!data) return '-';
    const [ano, mes, dia] = data.substring(0, 10).split('-');
    return `${dia}/${mes}/${ano}`;
}

// "2026-11-20T16:25:51" -> "20/11/2026 16:25"
export function formatarDataHora(dataHora) {
    if (!dataHora) return '-';
    return new Date(dataHora).toLocaleString('pt-BR', { dateStyle: 'short', timeStyle: 'short' });
}

// 42000 -> "42.000"
export function formatarNumero(numero) {
    if (numero === null || numero === undefined) return '-';
    return Number(numero).toLocaleString('pt-BR');
}

// Lista de UFs para os campos de endereço
export const estados = ['AC', 'AL', 'AP', 'AM', 'BA', 'CE', 'DF', 'ES', 'GO', 'MA', 'MT', 'MS', 'MG',
    'PA', 'PB', 'PR', 'PE', 'PI', 'RJ', 'RN', 'RS', 'RO', 'RR', 'SC', 'SP', 'SE', 'TO'];
