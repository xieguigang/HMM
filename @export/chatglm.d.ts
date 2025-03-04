// export R# package module type define for javascript/typescript language
//
//    imports "chatglm" from "Agent";
//
// ref=Agent.ChatGLM@Agent, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * 
*/
declare namespace chatglm {
   /**
    * create json data for run chatglm batch task
    * 
    * > the required file extension suffix name by upload 
    * >  batch data from web must be ``jsonl``!
    * 
     * @param content a dataframe object that contains the batch task data for run:
     *  
     *  1. id: the unique reference id of each task
     *  2. term: the term for do translation
     * @param prompt_text 
     * + default value Is ``'你是一个从英文到中文的语言翻译器'``.
     * @param add_explains 
     * + default value Is ``true``.
     * @param env 
     * + default value Is ``null``.
     * @return a collection of the batch task data for run the request
   */
   function batch_transaltion(content: object, prompt_text?: string, add_explains?: boolean, env?: object): object;
   /**
    * build json string for chatglm history input
    * 
    * 
     * @param his -
   */
   function history_json(his: object): string;
   /**
    * add a new record of chat input and ai response for create the history data
    * 
    * 
     * @param his -
     * @param input -
     * @param response -
   */
   function input_and_response(his: object, input: string, response: string): object;
   /**
    * parse the result of the chatglm batch request
    * 
    * 
     * @param file -
     * @param parse_annotation 
     * + default value Is ``true``.
     * @param env -
     * 
     * + default value Is ``null``.
   */
   function parse_batch_output(file: any, parse_annotation?: boolean, env?: object): any;
}
