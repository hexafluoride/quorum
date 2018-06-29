PGDMP     +                    v            quorum    10.3    10.3 6    A           0    0    ENCODING    ENCODING        SET client_encoding = 'UTF8';
                       false            B           0    0 
   STDSTRINGS 
   STDSTRINGS     (   SET standard_conforming_strings = 'on';
                       false            C           0    0 
   SEARCHPATH 
   SEARCHPATH     8   SELECT pg_catalog.set_config('search_path', '', false);
                       false            D           1262    16395    quorum    DATABASE     �   CREATE DATABASE quorum WITH TEMPLATE = template0 ENCODING = 'UTF8' LC_COLLATE = 'English_United States.1252' LC_CTYPE = 'English_United States.1252';
    DROP DATABASE quorum;
             postgres    false                        2615    2200    public    SCHEMA        CREATE SCHEMA public;
    DROP SCHEMA public;
             postgres    false            E           0    0    SCHEMA public    COMMENT     6   COMMENT ON SCHEMA public IS 'standard public schema';
                  postgres    false    3                        3079    12924    plpgsql 	   EXTENSION     ?   CREATE EXTENSION IF NOT EXISTS plpgsql WITH SCHEMA pg_catalog;
    DROP EXTENSION plpgsql;
                  false            F           0    0    EXTENSION plpgsql    COMMENT     @   COMMENT ON EXTENSION plpgsql IS 'PL/pgSQL procedural language';
                       false    1            k           1247    16473    board_parent_type    TYPE     K   CREATE TYPE public.board_parent_type AS ENUM (
    'group',
    'board'
);
 $   DROP TYPE public.board_parent_type;
       public       postgres    false    3            �            1259    16527    board_groups_id_seq    SEQUENCE     {   CREATE SEQUENCE public.board_groups_id_seq
    START WITH 1
    INCREMENT BY 1
    MINVALUE 0
    NO MAXVALUE
    CACHE 1;
 *   DROP SEQUENCE public.board_groups_id_seq;
       public       quorum_user    false    3            �            1259    16477    board_groups    TABLE     �   CREATE TABLE public.board_groups (
    id bigint DEFAULT nextval('public.board_groups_id_seq'::regclass) NOT NULL,
    name character varying NOT NULL,
    description character varying,
    parent bigint DEFAULT '-1'::integer
);
     DROP TABLE public.board_groups;
       public         quorum_user    false    206    3            �            1259    16525    boards_id_seq    SEQUENCE     u   CREATE SEQUENCE public.boards_id_seq
    START WITH 1
    INCREMENT BY 1
    MINVALUE 0
    NO MAXVALUE
    CACHE 1;
 $   DROP SEQUENCE public.boards_id_seq;
       public       quorum_user    false    3            �            1259    16464    boards    TABLE     1  CREATE TABLE public.boards (
    id bigint DEFAULT nextval('public.boards_id_seq'::regclass) NOT NULL,
    name character varying(100) NOT NULL,
    description character varying(1000) NOT NULL,
    parent_type public.board_parent_type NOT NULL,
    parent bigint NOT NULL,
    alias character varying
);
    DROP TABLE public.boards;
       public         quorum_user    false    205    619    3            �            1259    16396    logins    TABLE     �   CREATE TABLE public.logins (
    username character varying(50) NOT NULL,
    password character varying(200) NOT NULL,
    email character varying(200)
);
    DROP TABLE public.logins;
       public         quorum_user    false    3            �            1259    16529    posts_id_seq    SEQUENCE     t   CREATE SEQUENCE public.posts_id_seq
    START WITH 1
    INCREMENT BY 1
    MINVALUE 0
    NO MAXVALUE
    CACHE 1;
 #   DROP SEQUENCE public.posts_id_seq;
       public       quorum_user    false    3            �            1259    16436    posts    TABLE     �  CREATE TABLE public.posts (
    id bigint DEFAULT nextval('public.posts_id_seq'::regclass) NOT NULL,
    author bigint NOT NULL,
    thread bigint NOT NULL,
    content character varying NOT NULL,
    content_type character varying NOT NULL,
    created date NOT NULL,
    board bigint NOT NULL,
    rendered_content character varying NOT NULL,
    title character varying NOT NULL
);
    DROP TABLE public.posts;
       public         quorum_user    false    207    3            �            1259    16401    sessions    TABLE     �   CREATE TABLE public.sessions (
    id character varying(128) NOT NULL,
    created date NOT NULL,
    expires date,
    uid bigint NOT NULL
);
    DROP TABLE public.sessions;
       public         quorum_user    false    3            �            1259    16531    threads_id_seq    SEQUENCE     v   CREATE SEQUENCE public.threads_id_seq
    START WITH 1
    INCREMENT BY 1
    MINVALUE 0
    NO MAXVALUE
    CACHE 1;
 %   DROP SEQUENCE public.threads_id_seq;
       public       quorum_user    false    3            �            1259    16485    threads    TABLE     �   CREATE TABLE public.threads (
    id bigint DEFAULT nextval('public.threads_id_seq'::regclass) NOT NULL,
    board bigint NOT NULL,
    opening_post bigint,
    last_post bigint NOT NULL,
    title character varying NOT NULL
);
    DROP TABLE public.threads;
       public         quorum_user    false    208    3            �            1259    16446    user_map    TABLE     �   CREATE TABLE public.user_map (
    method character varying NOT NULL,
    identifier character varying NOT NULL,
    uid bigint NOT NULL
);
    DROP TABLE public.user_map;
       public         quorum_user    false    3            �            1259    16452    users    TABLE     �   CREATE TABLE public.users (
    uid bigint NOT NULL,
    username character varying(50) NOT NULL,
    bio character varying(500)
);
    DROP TABLE public.users;
       public         quorum_user    false    3            �            1259    16515    users_uid_seq    SEQUENCE     u   CREATE SEQUENCE public.users_uid_seq
    START WITH 1
    INCREMENT BY 1
    MINVALUE 0
    NO MAXVALUE
    CACHE 1;
 $   DROP SEQUENCE public.users_uid_seq;
       public       quorum_user    false    3    200            G           0    0    users_uid_seq    SEQUENCE OWNED BY     ?   ALTER SEQUENCE public.users_uid_seq OWNED BY public.users.uid;
            public       quorum_user    false    204            �
           2604    16517 	   users uid    DEFAULT     f   ALTER TABLE ONLY public.users ALTER COLUMN uid SET DEFAULT nextval('public.users_uid_seq'::regclass);
 8   ALTER TABLE public.users ALTER COLUMN uid DROP DEFAULT;
       public       quorum_user    false    204    200            8          0    16477    board_groups 
   TABLE DATA               E   COPY public.board_groups (id, name, description, parent) FROM stdin;
    public       quorum_user    false    202   �9       7          0    16464    boards 
   TABLE DATA               S   COPY public.boards (id, name, description, parent_type, parent, alias) FROM stdin;
    public       quorum_user    false    201   �9       2          0    16396    logins 
   TABLE DATA               ;   COPY public.logins (username, password, email) FROM stdin;
    public       quorum_user    false    196   B:       4          0    16436    posts 
   TABLE DATA               s   COPY public.posts (id, author, thread, content, content_type, created, board, rendered_content, title) FROM stdin;
    public       quorum_user    false    198   >;       3          0    16401    sessions 
   TABLE DATA               =   COPY public.sessions (id, created, expires, uid) FROM stdin;
    public       quorum_user    false    197   [;       9          0    16485    threads 
   TABLE DATA               L   COPY public.threads (id, board, opening_post, last_post, title) FROM stdin;
    public       quorum_user    false    203   x;       5          0    16446    user_map 
   TABLE DATA               ;   COPY public.user_map (method, identifier, uid) FROM stdin;
    public       quorum_user    false    199   �;       6          0    16452    users 
   TABLE DATA               3   COPY public.users (uid, username, bio) FROM stdin;
    public       quorum_user    false    200   �;       H           0    0    board_groups_id_seq    SEQUENCE SET     A   SELECT pg_catalog.setval('public.board_groups_id_seq', 2, true);
            public       quorum_user    false    206            I           0    0    boards_id_seq    SEQUENCE SET     ;   SELECT pg_catalog.setval('public.boards_id_seq', 3, true);
            public       quorum_user    false    205            J           0    0    posts_id_seq    SEQUENCE SET     :   SELECT pg_catalog.setval('public.posts_id_seq', 0, true);
            public       quorum_user    false    207            K           0    0    threads_id_seq    SEQUENCE SET     <   SELECT pg_catalog.setval('public.threads_id_seq', 0, true);
            public       quorum_user    false    208            L           0    0    users_uid_seq    SEQUENCE SET     ;   SELECT pg_catalog.setval('public.users_uid_seq', 7, true);
            public       quorum_user    false    204            �
           2606    16484    board_groups board_groups_pkey 
   CONSTRAINT     \   ALTER TABLE ONLY public.board_groups
    ADD CONSTRAINT board_groups_pkey PRIMARY KEY (id);
 H   ALTER TABLE ONLY public.board_groups DROP CONSTRAINT board_groups_pkey;
       public         quorum_user    false    202            �
           2606    16471    boards boards_pkey 
   CONSTRAINT     P   ALTER TABLE ONLY public.boards
    ADD CONSTRAINT boards_pkey PRIMARY KEY (id);
 <   ALTER TABLE ONLY public.boards DROP CONSTRAINT boards_pkey;
       public         quorum_user    false    201            �
           2606    16443    posts posts_pkey 
   CONSTRAINT     N   ALTER TABLE ONLY public.posts
    ADD CONSTRAINT posts_pkey PRIMARY KEY (id);
 :   ALTER TABLE ONLY public.posts DROP CONSTRAINT posts_pkey;
       public         quorum_user    false    198            �
           2606    16408    sessions sessions_pkey 
   CONSTRAINT     T   ALTER TABLE ONLY public.sessions
    ADD CONSTRAINT sessions_pkey PRIMARY KEY (id);
 @   ALTER TABLE ONLY public.sessions DROP CONSTRAINT sessions_pkey;
       public         quorum_user    false    197            �
           2606    16489    threads threads_pkey 
   CONSTRAINT     R   ALTER TABLE ONLY public.threads
    ADD CONSTRAINT threads_pkey PRIMARY KEY (id);
 >   ALTER TABLE ONLY public.threads DROP CONSTRAINT threads_pkey;
       public         quorum_user    false    203            �
           2606    16459    users users_pkey 
   CONSTRAINT     O   ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_pkey PRIMARY KEY (uid);
 :   ALTER TABLE ONLY public.users DROP CONSTRAINT users_pkey;
       public         quorum_user    false    200            �
           1259    16445    board    INDEX     8   CREATE INDEX board ON public.posts USING btree (board);
    DROP INDEX public.board;
       public         quorum_user    false    198            �
           1259    16512    fki_author_fkey_constraint    INDEX     N   CREATE INDEX fki_author_fkey_constraint ON public.posts USING btree (author);
 .   DROP INDEX public.fki_author_fkey_constraint;
       public         quorum_user    false    198            �
           1259    16501    fki_board_fkey_constraint    INDEX     N   CREATE INDEX fki_board_fkey_constraint ON public.threads USING btree (board);
 -   DROP INDEX public.fki_board_fkey_constraint;
       public         quorum_user    false    203            �
           1259    16523    fki_last_post_fkey_constraint    INDEX     V   CREATE INDEX fki_last_post_fkey_constraint ON public.threads USING btree (last_post);
 1   DROP INDEX public.fki_last_post_fkey_constraint;
       public         quorum_user    false    203            �
           1259    16495    fki_po    INDEX     B   CREATE INDEX fki_po ON public.threads USING btree (opening_post);
    DROP INDEX public.fki_po;
       public         quorum_user    false    203            �
           1259    16444    thread    INDEX     :   CREATE INDEX thread ON public.posts USING btree (thread);
    DROP INDEX public.thread;
       public         quorum_user    false    198            �
           2606    16507    posts author_fkey_constraint    FK CONSTRAINT     {   ALTER TABLE ONLY public.posts
    ADD CONSTRAINT author_fkey_constraint FOREIGN KEY (author) REFERENCES public.users(uid);
 F   ALTER TABLE ONLY public.posts DROP CONSTRAINT author_fkey_constraint;
       public       quorum_user    false    200    198    2730            �
           2606    16496    threads board_fkey_constraint    FK CONSTRAINT     {   ALTER TABLE ONLY public.threads
    ADD CONSTRAINT board_fkey_constraint FOREIGN KEY (board) REFERENCES public.boards(id);
 G   ALTER TABLE ONLY public.threads DROP CONSTRAINT board_fkey_constraint;
       public       quorum_user    false    2732    201    203            �
           2606    16502    posts board_fkey_constraint    FK CONSTRAINT     y   ALTER TABLE ONLY public.posts
    ADD CONSTRAINT board_fkey_constraint FOREIGN KEY (board) REFERENCES public.boards(id);
 E   ALTER TABLE ONLY public.posts DROP CONSTRAINT board_fkey_constraint;
       public       quorum_user    false    2732    198    201            �
           2606    16518 !   threads last_post_fkey_constraint    FK CONSTRAINT     �   ALTER TABLE ONLY public.threads
    ADD CONSTRAINT last_post_fkey_constraint FOREIGN KEY (last_post) REFERENCES public.posts(id);
 K   ALTER TABLE ONLY public.threads DROP CONSTRAINT last_post_fkey_constraint;
       public       quorum_user    false    198    203    2727            �
           2606    16490    threads op_fkey_constraint    FK CONSTRAINT     ~   ALTER TABLE ONLY public.threads
    ADD CONSTRAINT op_fkey_constraint FOREIGN KEY (opening_post) REFERENCES public.posts(id);
 D   ALTER TABLE ONLY public.threads DROP CONSTRAINT op_fkey_constraint;
       public       quorum_user    false    198    2727    203            8   Z   x�3�tO�K-J�Qp�,N.-.������,V ��ԢT�t�����Ă�Լb=N]C.cNǼ��J��Ģ�����?�\� �� 3      7   G   x�3�I-.Qp�O,J39Ӌ�K89c��� ���I�I`���Xޘ�1/�$#���#Yw� �5�      2   �   x�]�MO�0 ��3|nK/���	BnJ����x���Җ2
���E/&{��/O�gD�(�����}тX�i,�¤y��/q���KD[E% Z�i`�ǎR�	��Hq/��@�缒����*����;���{S���\��ә���_n�+Q$op��rwK'��F�� %���"��RL�0c���3i�����)۰�Na�����ֱO�K���Fc�ҋ4�����1P,�7�?uUU �\�      4      x������ � �      3      x������ � �      9      x������ � �      5   >   x���ON���H�HL�)�/�LI�4��V�s�B���y����fPnqIjNNb�9W�  Wz      6   L   x�3��H�HL�)�/�LI��H���W(�H-JU�2�,I-.���2���f���y��� �9gq	Pub����� �uS     