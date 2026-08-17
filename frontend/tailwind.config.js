/** @type {import('tailwindcss').Config} */
export default {
  content: ["./index.html", "./src/**/*.{js,ts,jsx,tsx}"],
  theme: { extend: { colors: { ink: "#16221d", brand: { 50: "#edf8f1", 100: "#d8efe0", 500: "#2f7d55", 600: "#286a49", 700: "#22563d" } }, boxShadow: { card: "0 1px 2px rgba(15,23,42,.04), 0 8px 24px rgba(15,23,42,.05)" } } },
  plugins: [],
}
