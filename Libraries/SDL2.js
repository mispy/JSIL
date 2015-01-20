"use strict";

if (typeof (JSIL) === "undefined") 
  throw new Error("JSIL.Core required");

/*var $jsilsdl = JSIL.DeclareAssembly("JSIL.SDL");

JSIL.DeclareNamespace("JSIL");
JSIL.DeclareNamespace("JSIL.SDL");*/

JSIL.ImplementExternals("SDL2.SDL", function ($) {
  $.Method({Static: true, Public: true}, "SDL_GetPlatform",
    new JSIL.MethodSignature($.String, [], []),
    function SDL_GetPlatform() {
      return "JSIL";
    }
  );
});
