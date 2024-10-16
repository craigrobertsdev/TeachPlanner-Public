/** @type {import('tailwindcss').Config} */

const config = {
  darkMode: ["class"],
  content: ["./app/**/*.{js,ts,jsx,tsx,mdx}", "./pages/**/*.{js,ts,jsx,tsx,mdx}", "./components/**/*.{js,ts,jsx,tsx,mdx}"],
  theme: {
    extend: {
      colors: {
        background: "hsl(var(--background))",
        foreground: "hsl(var(--foreground))",
        base: "#F5F5F7",
        main: "#EEE6DE",
        sage: "#90AEB2",
        darkGreen: "#37514D",
        peach: "#DD8E75",
        ceramic: "#B6594C",
        loading: "#00000080",
        cosmos: "#550C18",
        lightPeach: "#DD8E7550",
        lightSage: "#90AEB250",
        baseHover: "#CFCAC5",
        sageHover: "#A3C1C5",
        darkGreenHover: "#2E3F3C",
        peachHover: "#C97C5D",
        ceramicHover: "#A84F42",
        baseFocus: "#7F7D7B",
        sageFocus: "#6D9EA2",
        baseFocusBorder: "#999188",
        darkGreenBorder: "#37514D55",
        baseDisabled: "#F2EDE9",
        sageDisabled: "#D1E0E2",
        darkGreenDisabled: "#5C6D6A",
        peachDisabled: "#E3B8A9",
        ceramicDisabled: "#D08F7F",
        mathematics: "#FCCF0344",
        english: "#85D42A44",
        humanitiesAndSocialScience: "#2AD48D44",
        healthAndPhysicalEducation: "#AD3BFF44",
        science: "#FF3B3B44",
        german: "#2AD48D44",
        dance: "#AD3BFF44",
        drama: "#FF3B3B44",
        nit: "#42E3F544",
        card: {
          default: "hsl(var(--card))",
          foreground: "hsl(var(--card-foreground))",
        },
        popover: {
          DEFAULT: "hsl(var(--popover))",
          foreground: "hsl(var(--popover-foreground))",
        },
        primary: {
          DEFAULT: "hsl(var(--primary))",
          foreground: "hsl(var(--primary-foreground))",
        },
        secondary: {
          DEFAULT: "hsl(var(--secondary))",
          foreground: "hsl(var(--secondary-foreground))",
        },
        muted: {
          DEFAULT: "hsl(var(--muted))",
          foreground: "hsl(var(--muted-foreground))",
        },
        accent: {
          DEFAULT: "hsl(var(--accent))",
          foreground: "hsl(var(--accent-foreground))",
        },
        destructive: {
          DEFAULT: "hsl(var(--destructive))",
          foreground: "hsl(var(--destructive-foreground))",
        },
        border: "hsl(var(--border))",
        input: "hsl(var(--input))",
        ring: "hsl(var(--ring))",
        chart: {
          1: "hsl(var(--chart-1))",
          2: "hsl(var(--chart-2))",
          3: "hsl(var(--chart-3))",
          4: "hsl(var(--chart-4))",
          5: "hsl(var(--chart-5))",
        },
      },
      borderRadius: {
        lg: "var(--radius)",
        md: "calc(var(--radius) - 2px)",
        sm: "calc(var(--radius) - 4px)",
      },
    },
  },
  plugins: [require("tailwindcss-animate")],
};
export default config;
