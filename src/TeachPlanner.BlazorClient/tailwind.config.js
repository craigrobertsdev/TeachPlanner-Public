/** @type {import('tailwindcss').Config} */
module.exports = {
  content: ["./wwwroot/index.html", "./Pages/**/*.{razor,html,cshtml}", "./Layout/**/*.{razor,html,cshtml}", "./Components/**/*.{razor,html,cshtml}"],
  plugins: [],
  theme: {
    extend: {
      colors: {
        // core colors
        base: "#F5F5F7",
        main: "#EEE6DE",
        sage: "#90AEB2",
        darkGreen: "#37514D",
        peach: "#DD8E75",
        ceramic: "#B6594C",
        loading: "#00000080",
        cosmos: "#550C18",

        // light
        lightPeach: "#DD8E7550",
        lightSage: "#90AEB250",

        // hover
        baseHover: "#CFCAC5",
        sageHover: "#A3C1C5",
        darkGreenHover: "#2E3F3C",
        peachHover: "#C97C5D",
        ceramicHover: "#A84F42",
        // focus
        baseFocus: "#7F7D7B",
        sageFocus: "#6D9EA2",

        // border
        baseFocusBorder: "#999188",
        darkGreenBorder: "#37514D55",

        // disabled
        baseDisabled: "#F2EDE9",
        sageDisabled: "#D1E0E2",
        darkGreenDisabled: "#5C6D6A",
        peachDisabled: "#E3B8A9",
        ceramicDisabled: "#D08F7F",

        // curriculum
        mathematics: "#FCCF0344",
        english: "#85D42A44",
        humanitiesAndSocialScience: "#2AD48D44",
        healthAndPhysicalEducation: "#AD3BFF44",
        science: "#FF3B3B44",
        german: "#2AD48D44",
        dance: "#AD3BFF44",
        drama: "#FF3B3B44",

        //other
        nit: "#42E3F544",
      },
      height: {
        screen: "100dvh",
        "80%": "80%",
      },
      maxWidth: {
        "80ch": "80ch",
      },
      minHeight: {
        screen: "100dvh",
      },
      gridRowStart: {
        8: "8",
        9: "9",
        10: "10",
        11: "11",
        12: "12",
      },
    },
    fontFamily: {
      theme: ["'Nunito Sans'", "sans-serif"],
    },
    transitionProperty: {
      "font-size": "font-size",
    },
  },
  safelist: [
    "max-w-80ch",
    {
      pattern: /row-start-(1|2|3|4|5|6|7|8|9|10|11|12)/,
    },
    {
      pattern: /col-start-(1|2|3|4|5|6|7|8|9|10)/,
    },
    {
      pattern: /row-span-(1|2|3|4|5|6|7|8)/,
    },
    {
      pattern:
        /(bg|text|border)-(nit|sage|mathematics|english|science|humanitiesAndSocialScience|healthAndPhysicalEducation|dance|drama|german|darkGreenBorder)/,
    },
    {
      pattern: /grid-rows-[^\s]+/,
    },
    {
      pattern: /(mt|mb|mr|ml|my|mx|px|py|pt|pb|pl|pr)-[0-9]+/,
    },
    {
      pattern: /flex-.*/,
    },
    {
      pattern: /(bottom|right|top|left)-[0-9]+/,
    },
    {
      pattern: /(w|h)-[0-9]+/,
    },
    {
      pattern: /text-(right|center)/,
    },
  ],
};
