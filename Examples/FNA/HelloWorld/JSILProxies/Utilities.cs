using System;
using JSIL.Meta;
using JSIL.Proxy;

namespace HelloWorld.JSILProxies {
    [JSProxy(
        "MonoGame.Utilities.AssemblyHelper",
        JSProxyMemberPolicy.ReplaceDeclared,
        JSProxyAttributePolicy.ReplaceDeclared,
        JSProxyInterfacePolicy.ReplaceDeclared
    )]
    public abstract class AssemblyHelperProxy {
        [JSReplacement("document.title")]
        public static string GetDefaultWindowTitle() {
            throw new NotImplementedException();
        }
    }
}
