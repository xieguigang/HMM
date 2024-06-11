// export R# package module type define for javascript/typescript language
//
//    imports "chatglm" from "Agent";
//
// ref=Agent.ChatGLM@Agent, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace chatglm {
   /**
     * @param env default value Is ``null``.
   */
   function batch_transaltion(content: object, env?: object): object;
   /**
   */
   function history_json(his: object): string;
   /**
   */
   function input_and_response(his: object, input: string, response: string): object;
   /**
     * @param parse_annotation default value Is ``true``.
     * @param env default value Is ``null``.
   */
   function parse_batch_output(file: any, parse_annotation?: boolean, env?: object): any;
}
