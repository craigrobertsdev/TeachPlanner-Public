"use client";
import { Button } from "@/components/ui/button";
import Image from "next/image";

function toggleTheme() {
  document.documentElement.classList.toggle("dark");
}

export default function Home() {
  return (
    <>
      <div className="flex gap-3 p-2">
        <Button variant="default" onClick={toggleTheme}>
          Toggle
        </Button>
        <Button variant="secondary" onClick={toggleTheme}>
          Toggle
        </Button>
        <Button variant="outline" onClick={toggleTheme}>
          Toggle
        </Button>
        <Button variant="link" onClick={toggleTheme}>
          Toggle
        </Button>
        <Button variant="destructive" onClick={toggleTheme}>
          Toggle
        </Button>
        <Button variant="ghost" onClick={toggleTheme}>
          Toggle
        </Button>
      </div>
      <h1 className="text-2xl">This is some text to see what it looks like with different themes</h1>
    </>
  );
}
