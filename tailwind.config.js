const colors = require('tailwindcss/colors')

module.exports = {
    safelist: [],
    content: [
        "./**/*.razor",
        "./wwwroot/index.html",
        "./Pages/**/*.razor",
        "./Shared/**/*.razor"
    ],
    theme: {
        container: {
          center: true,
          padding: '1rem'  
        },
        colors: {
            transparent: 'transparent',
            current: 'currentColor',
            black: colors.black,
            white: colors.white,
            gray: colors.stone,
            yellow: colors.yellow,
            primary: {
                50:  "#fef6f8",
                100: "#fdecef",
                200: "#fbd1da",
                300: "#f7a7b8",
                400: "#f26f8f",
                500: "#ee3967",
                600: "#c92c52",
                700: "#9f203f",
                800: "#73162d",
                900: "#4b0d1d",
                950: "#2a060f",
            },
            accent: "#E1CE55"
        },
        extend: {
            borderRadius: {
                '3xl': '2rem',
            },
            fontFamily: {
                manrope: ["Manrope", 'sans-serif'],
            }
        },
    },
    plugins: [],
}
