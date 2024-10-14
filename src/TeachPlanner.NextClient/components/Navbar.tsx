"use client";

import { SignedIn, SignedOut, SignInButton, SignOutButton } from "@clerk/nextjs";
import { useTheme } from "next-themes";
import Link from "next/link";
import { SunIcon, MoonIcon } from "@radix-ui/react-icons";
import { Button } from "./ui/button";
import ProfileButton from "./ui/ProfileButton";

export const Navbar = () => {
  const { theme, setTheme } = useTheme();
  const routes = [
    {
      name: "Week Planner",
      href: "/week-planner",
    },
    {
      name: "Term Planner",
      href: "/term-planner",
    },
    {
      name: "Year Planner",
      href: "/year-planner",
    },
    {
      name: "Lesson Template Creator",
      href: "/lesson-template-creator",
    },
    {
      name: "Settings",
      href: "/settings",
    },
  ];
  return (
    <header className="bg-sage dark:bg-primary">
      <nav className="flex flex-none items-center p-1 w-full sm:text-sm">
        <Link href="/">
          <h1 className="md:text-xl lg:text-2xl border-r-[2px] pr-4 pl-2">Teach Planner</h1>
        </Link>
        <ul className="list-none flex flex-1 text-xs sm:text-sm md:text-md xl:text-xl items-center">
          {routes.map((route, i) => (
            <li key={`navitem-${i}`} className="border-r">
              <Button variant="ghost" className="text-xs sm:text-sm md:text-md xl:text-xl mx-1">
                <Link href={route.href}>{route.name}</Link>
              </Button>
            </li>
          ))}
          <li className="ml-auto pr-6">
            <SignOutButton />
          </li>
          <li>
            <Button
              variant="ghost"
              size="icon"
              aria-label="Toggle theme"
              className="mr-6"
              onClick={() => {
                console.log("theme", theme);
                setTheme(theme === "dark" ? "light" : "dark");
              }}>
              <SunIcon className="h-6 w-6 rotate-0 scale-100 transition-all dark:rotate-90 dark:scale-0" />
              <MoonIcon className="absolute h-6 w-6 rotate-90 scale-0 transition-all dark:rotate-0 dark:scale-100" />
              <span className="sr-only">Toggle Theme</span>
            </Button>
          </li>
          <li className="pr-4">
            <ProfileButton initials="CR" />
          </li>
        </ul>
      </nav>
    </header>
  );
};
