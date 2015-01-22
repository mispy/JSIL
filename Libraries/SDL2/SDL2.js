"use strict";

/* This is a wrapper to translate P/Invoke calls from C# to the equivalent functions in emscripten-compiled SDL2 */


if (typeof (JSIL) === "undefined") 
  throw new Error("JSIL.Core required");

// Not entirely sure how DeclareAssembly/Namespace works
/*var $jsilsdl = JSIL.DeclareAssembly("JSIL.SDL");

JSIL.DeclareNamespace("JSIL");
JSIL.DeclareNamespace("JSIL.SDL");*/


// TODO: make this not FNA-dependent (how does that work, since FNA embeds SDL2-CS into its own assembly?)
var $sdlasm = new JSIL.AssemblyCollection({
  fna: "FNA, Version=0.0.0.1, Culture=neutral, PublicKeyToken=null"
}).fna;

JSIL.DeclareNamespace("SDL2");

JSIL.ImplementExternals("SDL2.SDL", function ($) {
  $.Method({Static: true, Public: true}, "SDL_GetPlatform",
    new JSIL.MethodSignature($.String, [], []),
    function SDL_GetPlatform() {
      return Module.ccall("SDL_GetPlatform", 'string', [], []); // We should maybe be using cwrap instead of ccall to build these
    }
  );
});

JSIL.ImplementExternals("SDL2.SDL", function ($) {
  $.Method({Static: true, Public: true}, "SDL_SetWindowIcon",
    new JSIL.MethodSignature($.String, [], []),
    function SDL_SetWindowIcon() {
      return Module.ccall("SDL_SetWindowIcon", 'string', [], []); 
    }
  );
});

JSIL.ImplementExternals("SDL2.SDL", function ($) {
  $.Method({Static: true, Public: true}, "SDL_SetMainReady",
    new JSIL.MethodSignature(null, [], []),
    function SDL_SetMainReady() {
      return Module.ccall("SDL_SetMainReady", null, [], []); 
    }
  );
});

JSIL.ImplementExternals("SDL2.SDL", function ($) {
  $.Method({Static: true, Public: true}, "SDL_Init",
    new JSIL.MethodSignature($.Int32, [$.Int32], []),
    function SDL_Init(flags) {
      return Module.ccall("SDL_Init", 'number', ['number'], [flags]); 
    }
  );
});

JSIL.ImplementExternals("SDL2.SDL", function ($) {
  $.Method({Static: true, Public: true}, "SDL_GL_SetAttribute",
    new JSIL.MethodSignature($.Int32, [$sdlasm.TypeRef("SDL2.SDL_SDL_GLattr"), $.Int32], []),
    function SDL_GL_SetAttribute(attr, value) {
      return Module.ccall("SDL_GL_SetAttribute", 'number', ['number', 'number'], [attr, value]); 
    }
  );
});

JSIL.ImplementExternals("SDL2.SDL", function ($) {
  $.Method({Static: true, Public: true}, "SDL_CreateWindow",
    new JSIL.MethodSignature($jsilcore.TypeRef("System.IntPtr"), [$.String, $.Int32, $.Int32, $.Int32, $.Int32, $sdlasm.TypeRef("SDL2.SDL_SDL_WindowFlags")], []),
    function SDL_CreateWindow(title, x, y, w, h, flags) {
      return Module.ccall("SDL_CreateWindow", 'number', ['string', 'number', 'number', 'number', 'number', 'number'], [title, x, y, w, h, flags.GetHashCode()]); 
    }
  );
});
