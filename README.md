# UrlImageSaver
Application to download all images from entry url and save it local and to Postgre

## Parameters

### Entry URL
URL is passed through the application argument
```sh
UrlImageSaver www.google.com
```

### appsettings.json

#### Application section
| Param | Desc |
| ------ | ------ |
| UseLocal | Save images to local machine |
| UsePostgre | Save images to PostgreSql database |

#### Application:LocalStorage section
| Param | Desc |
| ------ | ------ |
| LocalPath | Destination folder path  |
| ShouldCreateFolder | Allow to create folder if not exist |

#### Application:PostgreStorage section
| Param | Desc |
| ------ | ------ |
| ConnectionString | Connection string to PostgreSql  |
| UseProcedure | Use procedure to save image otherwise table insert |
| ProcedureName | Procedure name to execute for |
| TableName | Table name to insert images for |

## Postgre infrastructure example

### Table

```sh
CREATE TABLE public."Images"
(
    id integer NOT NULL,
    name text NOT NULL,
    content bytea NOT NULL,
    PRIMARY KEY (id)
);
```

### Procedure

```sh
CREATE OR REPLACE FUNCTION public.SaveFile(p_name text, p_content bytea) RETURNS void AS $$
    BEGIN
      INSERT INTO public."Images" (name, content) VALUES (p_name, p_content);
    END;
$$ LANGUAGE plpgsql
```


