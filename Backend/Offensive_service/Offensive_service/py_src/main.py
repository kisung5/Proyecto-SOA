import sys
import obo
import PyPDF2

def file_ofen_avg_request(File_Name, Language):
    ###### creating a pdf file object #####
    if File_Name[-3:] == 'txt':
        File=open('test.txt','r') # open the file to read
        texto = File.read() # take the text
        File.close() # close the file
    elif File_Name[-3:] == 'pdf':
    #else: 
        pdfFileObj = open(File_Name, 'rb') # open the file
        pdfReader = PyPDF2.PdfFileReader(pdfFileObj) # creating a pdf reader object
        numPages = pdfReader.numPages # number of pages in pdf file
        texto = ""
        for i in range(numPages):
            pageObj = pdfReader.getPage(i)
            texto += pageObj.extractText()

    texto = texto.lower()

    if Language == 'en':
        listaPalabrasCompleta = obo.quitaNoAlfaNum(texto)
        total_words = len(listaPalabrasCompleta)
        listaPalabras = obo.quitarPalabrasvac(listaPalabrasCompleta, obo.palabrasvac)
        diccionario = obo.listaPalabrasDicFrec(listaPalabras)
        diccOrdenado = obo.ordenaDicFrec(diccionario)
        english_ofen_list = ['arse', 'ass', 'asshole', 'bastard', 'bitch', 'bollocks', 'brotherfucker', 'bugger',
                             'bullshit', 'child-fucker', 'christ on a bike', 'Christ on a cracker', 'cocksucker', 'crap',
                             'cunt', 'damn', 'effing', 'fatherfucker', 'frigger', 'fuck', 'goddamn', 'hell', 'holy shit',
                             'horse', 'idiot', 'stupid', 'shit', 'motherfucker', 'nigga', 'piss', 'prick', 'shit ass',
                             'shitass', 'sisterfucker', 'slut', 'son of a bitch', 'son of a whore', 'twat']
        ofen_count = 0
        for word in diccOrdenado:
            for ofen_word in english_ofen_list:
                if word[1] == ofen_word:
                    ofen_count += word[0]
                    
##    elif Language == 'es':
        ##print(texto)
        ##print(diccOrdenado)

    ##print("Palabras ofensivas encontradas: ", ofen_count)
    ##print("Total de palabras del doc: ", total_words)
    file_ofen_avg = (ofen_count / total_words)*100
    int_test = int(file_ofen_avg)
##    print('The offensive average of the file >>' + FileName + '<< is: ', file_ofen_avg)
    #print('The offensive average of the file is: ', file_ofen_avg, '% from 100%')
    return file_ofen_avg

if __name__ == '__main__':
    
    fileName = sys.argv[1]
    language = sys.argv[2]
    
    offensivePercentage = file_ofen_avg_request(fileName, language)
    
    print(offensivePercentage)
    
