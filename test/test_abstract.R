data_str = read.csv(file.path(@dir,"Methoxamine_datasheet.csv"), row.names = NULL, check.names = FALSE);
data_str = data_str$abstract;

print(data_str);

writeLines(data_str, con = file.path(@dir, "abstract.txt"));