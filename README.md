########################################
#   Welkom to The Feature Processor!   #
#        Made by: Anne van Ede         #
# ------------------------------------ #
#  This tool can extract feature info  #
#    from XML files and CSV files      #
########################################

##############################################################################################################################################################
usage																		:   [-function_option] [-p parameter parameter]
-h																			:   Help
            
Function options:
-extract_jsym [-p feature_definitions.xml extracted_feature_values.xml]		:   Extract features from JSymbolic
-print_all_xml [-p feature_info_values]										:   Print Feature info and values to XML
-print_inf_xml [-p feature_info]											:   Print Feature info to XML
-print_val_xml [-p feature_values]											:   Print Feature values to XML
-print_all_csv	[-p feature_info_values]									:   Print Feature info and values to CSV
-change_basedir [-p C:/]												:   View and change base directory

Leave parameter part ([-p ...]) away for default values as show above
NB: no spaces in filenames allowed.
##############################################################################################################################################################

Examples:

1> -extract_jsym
2> -extract_jsym -p def.xml val.xml

3> change -p D:/username/foldername 