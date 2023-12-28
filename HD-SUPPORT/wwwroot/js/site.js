// script.js

document.addEventListener('DOMContentLoaded', function () {
    const sideMenu = document.getElementById('sideMenu');
    const iconMenu = document.querySelector('.icon_menu');
    const closeMenuButton = document.querySelector('.close_menu');

    // Adiciona o evento de clique ao ícone do menu
    iconMenu.addEventListener('click', function (event) {
        event.stopPropagation(); // Impede que o clique se propague para o documento
        toggleMenu();
    });

    // Adiciona o evento de clique ao botão de fechar menu
    closeMenuButton.addEventListener('click', function () {
        toggleMenu();
    });

    // Fecha o menu se clicar fora dele
    document.addEventListener('click', function (event) {
        const isClickInsideMenu = sideMenu.contains(event.target);
        const isClickOnIcon = event.target === iconMenu;

        if (!isClickInsideMenu && !isClickOnIcon) {
            closeMenu();
        }
    });

    // Função para abrir o menu
    function openMenu() {
        sideMenu.style.right = '0px';
    }

    // Função para fechar o menu
    function closeMenu() {
        sideMenu.style.right = '-300px';
    }

    // Função para alternar entre abrir/fechar o menu
    function toggleMenu() {
        const isMenuOpen = sideMenu.style.right === '0px';
        if (isMenuOpen) {
            closeMenu();
        } else {
            openMenu();
        }
    }
});