/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  darkMode: 'class',
  theme: {
    extend: {
      colors: {
        primary: {
          50: '#f0f4ff',
          100: '#d9e2ff',
          200: '#bccaff',
          300: '#8da4ff',
          400: '#5871ff',
          500: '#3245ff',
          600: '#1e20ff',
          700: '#1917eb',
          800: '#1613bc',
          900: '#191894',
          950: '#100e57',
        },
        academic: {
          dark: '#0f172a',    // Slate 900
          gold: '#fbbf24',    // Amber 400
          silver: '#94a3b8',  // Slate 400
        }
      },
      fontFamily: {
        sans: ['Inter', 'system-ui', 'sans-serif'],
      },
    },
  },
  plugins: [],
}
