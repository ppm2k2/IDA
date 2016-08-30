using System;
using System.IO;
using DidiSoft.Pgp;
using DidiSoft.Pgp.Exceptions;

namespace ConceptONE.Infrastructure.Encryption
{
    public class PGPEncryptLib
    {
        public PGPEncryptLib()
        {

        }

        public string DecryptFile(string EncryptedFilePath, string KeyInboundPath, string KeyInboundPassword, string SenderKeyPath, string OutputFilePath)
        {
            string result = null;

            // initialize the library
            PGPLib pgp = new PGPLib();
            pgp.Cypher = CypherAlgorithm.TRIPLE_DES;

            try
            {
                bool verified = pgp.DecryptAndVerifyFile(EncryptedFilePath,
                                                     KeyInboundPath,
                                                     KeyInboundPassword,
                                                     SenderKeyPath,
                                                     OutputFilePath);

                if (!verified)
                {
                    throw new Exception("Signature is invalid!");
                }

            }
            catch (IOException ex)
            {
                throw ex;
            }
            catch (PGPException ex)
            {
                if (ex is NonPGPDataException)
                {
                    throw new Exception("the passed encrypted input is not a valid OpenPGP archive");
                }
                else if (ex is IntegrityCheckException)
                {
                    throw new Exception("the passed encrypted input is corrupted");
                }
                else if (ex is FileIsPBEEncryptedException)
                {
                    throw new Exception("the passed encrypted input is encrypted with a password, but an attempted was made to decrypt it with a private key");
                }
                else if (ex is WrongPrivateKeyException)
                {
                    throw new Exception("the encrypted input was encrypted with a different private key than the provided one");
                }
                else if (ex is WrongPasswordException)
                {
                    throw new Exception("the password for the provided private key is wrong");
                }
                else
                {
                    throw new Exception("general decryption error");
                }
            }

            return result;

        }

        public string DecryptFiles(string EncryptedFilePath, string KeyInboundPath, string KeyInboundPassword, string OutputFolder)
        {
            string result = null;

            // initialize the library
            PGPLib pgp = new PGPLib();
            pgp.Cypher = CypherAlgorithm.TRIPLE_DES;

            // decrypts the content of the PGP archive
            // and returns array with full file paths of the 
            // exytracted file(s)
            string[] decryptedFileNames = { };

            try
            {
                decryptedFileNames =
                    pgp.DecryptTo(EncryptedFilePath,
                                KeyInboundPath,
                                KeyInboundPassword,
                                OutputFolder);
            }
            catch (IOException ex)
            {
                throw ex;
            }
            catch (PGPException ex)
            {
                if (ex is NonPGPDataException)
                {
                    throw new Exception("the passed encrypted input is not a valid OpenPGP archive");
                }
                else if (ex is IntegrityCheckException)
                {
                    throw new Exception("the passed encrypted input is corrupted");
                }
                else if (ex is FileIsPBEEncryptedException)
                {
                    throw new Exception("the passed encrypted input is encrypted with a password, but an attempted was made to decrypt it with a private key");
                }
                else if (ex is WrongPrivateKeyException)
                {
                    throw new Exception("the encrypted input was encrypted with a different private key than the provided one");
                }
                else if (ex is WrongPasswordException)
                {
                    throw new Exception("the password for the provided private key is wrong");
                }
                else
                {
                    throw new Exception("general decryption error");
                }
            }

            return result;

        }


    }
}
