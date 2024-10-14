import type { Metadata } from "next";
import localFont from "next/font/local";
import "./globals.css";
import { Navbar } from "@/components/Navbar";
import { ClerkProvider, SignedIn, SignedOut, SignInButton, SignOutButton } from "@clerk/nextjs";
import { ThemeProvider } from "@/components/ui/ThemeProvider";

const geistSans = localFont({
  src: "./fonts/GeistVF.woff",
  variable: "--font-geist-sans",
  weight: "100 900",
});
const geistMono = localFont({
  src: "./fonts/GeistMonoVF.woff",
  variable: "--font-geist-mono",
  weight: "100 900",
});

export const metadata: Metadata = {
  title: {
    template: "%s | TeachPlanner",
    default: "TeachPlanner",
  },
  description: "An all-in-one tool for teachers to plan lessons, manage resources, and track student progress.",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <ClerkProvider>
      <html lang="en" className="h-[100dvh] min-h-[100dvh]">
        <body suppressHydrationWarning className={`${geistSans.variable} ${geistMono.variable} antialiased h-full min-h-full`}>
          <div className="h-screen relative flex flex-col">
            <ThemeProvider attribute="class">
              <SignedIn>
                <Navbar />
              </SignedIn>
              <div className="flex-grow">{children}</div>
            </ThemeProvider>
          </div>
        </body>
      </html>
    </ClerkProvider>
  );
}
