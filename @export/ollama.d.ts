// export R# package module type define for javascript/typescript language
//
//    imports "ollama" from "Agent";
//
// ref=Agent.OLlamaDemo@Agent, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace ollama {
   /**
     * @param args default value Is ``null``.
     * @param fcall default value Is ``null``.
     * @param env default value Is ``null``.
   */
   function add_tool(model: object, name: string, desc: string, requires: any, args?: object, fcall?: any, env?: object): any;
   /**
   */
   function chat(model: object, msg: string): any;
   /**
     * @param ollama_serve default value Is ``'127.0.0.1:11434'``.
     * @param model default value Is ``'deepseek-r1:671b'``.
   */
   function deepseek_chat(message: string, ollama_serve?: string, model?: string): object;
   /**
     * @param ollama_server default value Is ``'127.0.0.1:11434'``.
   */
   function new(model: string, ollama_server?: string): object;
}
